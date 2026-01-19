import { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import TransactionList from '../components/TransactionList';
import type { AccountDto, TransactionDto } from '../types';
import { accountsApi } from '../services/api';

export default function AccountDetails() {
    const { id } = useParams<{ id: string }>();
    const [account, setAccount] = useState<AccountDto | null>(null);
    const [transactions, setTransactions] = useState<TransactionDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [showModal, setShowModal] = useState<'deposit' | 'withdraw' | null>(null);
    const [amount, setAmount] = useState('');
    const [description, setDescription] = useState('');
    const [processing, setProcessing] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (id) {
            loadAccount();
            loadTransactions();
        }
    }, [id]);

    const loadAccount = async () => {
        try {
            const data = await accountsApi.getById(id!);
            setAccount(data);
        } catch (err) {
            console.error('Failed to load account:', err);
        } finally {
            setLoading(false);
        }
    };

    const loadTransactions = async () => {
        try {
            const data = await accountsApi.getTransactions(id!, 1, 20);
            setTransactions(data.items);
        } catch (err) {
            console.error('Failed to load transactions:', err);
        }
    };

    const handleTransaction = async () => {
        if (!amount || parseFloat(amount) <= 0) return;

        setProcessing(true);
        setError(null);

        try {
            if (showModal === 'deposit') {
                await accountsApi.deposit(id!, {
                    amount: parseFloat(amount),
                    description: description || 'Deposit',
                });
            } else {
                await accountsApi.withdraw(id!, {
                    amount: parseFloat(amount),
                    description: description || 'Withdrawal',
                });
            }

            // Refresh data
            await loadAccount();
            await loadTransactions();
            setShowModal(null);
            setAmount('');
            setDescription('');
        } catch (err: any) {
            setError(err.response?.data?.error || 'Transaction failed');
        } finally {
            setProcessing(false);
        }
    };

    const formatCurrency = (amount: number, currency: string) => {
        return new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: currency,
        }).format(amount);
    };

    if (loading) {
        return (
            <div className="flex items-center justify-center h-64">
                <div className="animate-spin rounded-full h-12 w-12 border-4 border-blue-500 border-t-transparent"></div>
            </div>
        );
    }

    if (!account) {
        return (
            <div className="text-center py-12">
                <h2 className="text-xl font-semibold text-gray-900">Account not found</h2>
                <Link to="/" className="text-blue-600 hover:underline mt-4 inline-block">
                    Back to Dashboard
                </Link>
            </div>
        );
    }

    return (
        <div>
            {/* Back Button */}
            <Link to="/" className="text-blue-600 hover:text-blue-800 flex items-center space-x-1 mb-6">
                <span>←</span>
                <span>Back to Dashboard</span>
            </Link>

            {/* Account Header */}
            <div className="bg-gradient-to-r from-blue-600 to-blue-800 rounded-2xl p-8 text-white mb-8 shadow-xl">
                <div className="flex justify-between items-start">
                    <div>
                        <span className="bg-white/20 px-3 py-1 rounded-full text-sm">{account.type}</span>
                        <h1 className="text-4xl font-bold mt-4">{formatCurrency(account.balance, account.currency)}</h1>
                        <p className="text-blue-200 mt-2">Account •••• {account.accountNumber.slice(-4)}</p>
                    </div>
                    <div className="flex space-x-3">
                        <button
                            onClick={() => setShowModal('deposit')}
                            className="bg-green-500 hover:bg-green-600 px-6 py-3 rounded-lg font-semibold shadow-lg transition-all"
                        >
                            + Deposit
                        </button>
                        <button
                            onClick={() => setShowModal('withdraw')}
                            className="bg-white text-blue-800 hover:bg-blue-50 px-6 py-3 rounded-lg font-semibold shadow-lg transition-all"
                        >
                            − Withdraw
                        </button>
                    </div>
                </div>
            </div>

            {/* Transactions */}
            <div className="bg-white rounded-2xl shadow-lg p-6">
                <h2 className="text-xl font-bold text-gray-900 mb-4">Recent Transactions</h2>
                <TransactionList transactions={transactions} />
            </div>

            {/* Transaction Modal */}
            {showModal && (
                <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
                    <div className="bg-white rounded-2xl p-8 max-w-md w-full mx-4 shadow-2xl">
                        <h2 className="text-2xl font-bold text-gray-900 mb-6">
                            {showModal === 'deposit' ? 'Deposit Funds' : 'Withdraw Funds'}
                        </h2>

                        <div className="space-y-4 mb-6">
                            <div>
                                <label className="block text-sm font-medium text-gray-700 mb-1">Amount</label>
                                <div className="relative">
                                    <span className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-500">$</span>
                                    <input
                                        type="number"
                                        value={amount}
                                        onChange={(e) => setAmount(e.target.value)}
                                        className="w-full pl-8 pr-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                                        placeholder="0.00"
                                        min="0"
                                        step="0.01"
                                    />
                                </div>
                            </div>
                            <div>
                                <label className="block text-sm font-medium text-gray-700 mb-1">Description (optional)</label>
                                <input
                                    type="text"
                                    value={description}
                                    onChange={(e) => setDescription(e.target.value)}
                                    className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                                    placeholder="e.g., Paycheck, ATM withdrawal"
                                />
                            </div>
                        </div>

                        {error && (
                            <div className="mb-4 p-3 bg-red-100 text-red-700 rounded-lg">
                                {error}
                            </div>
                        )}

                        <div className="flex space-x-4">
                            <button
                                onClick={() => { setShowModal(null); setError(null); }}
                                className="flex-1 px-4 py-3 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50"
                            >
                                Cancel
                            </button>
                            <button
                                onClick={handleTransaction}
                                disabled={processing || !amount}
                                className={`flex-1 px-4 py-3 rounded-lg font-semibold disabled:opacity-50 ${showModal === 'deposit'
                                        ? 'bg-green-500 hover:bg-green-600 text-white'
                                        : 'bg-red-500 hover:bg-red-600 text-white'
                                    }`}
                            >
                                {processing ? 'Processing...' : showModal === 'deposit' ? 'Deposit' : 'Withdraw'}
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}
