import { useState, useEffect } from 'react';
import AccountCard from '../components/AccountCard';
import type { AccountDto, CreateAccountRequest } from '../types';
import { accountsApi, usersApi } from '../services/api';
import { useUser } from '../context/UserContext';

export default function Dashboard() {
    const { user } = useUser();
    const [accounts, setAccounts] = useState<AccountDto[]>([]);
    const [loading, setLoading] = useState(false);
    const [initialLoading, setInitialLoading] = useState(true);
    const [showCreateModal, setShowCreateModal] = useState(false);
    const [selectedType, setSelectedType] = useState(1);
    const [error, setError] = useState<string | null>(null);

    const accountTypes = [
        { value: 1, label: 'Checking', description: 'For everyday transactions' },
        { value: 2, label: 'Savings', description: 'Earn interest on your balance' },
        { value: 3, label: 'Money Market', description: 'Higher interest rates' },
    ];

    // Load user's accounts on mount
    useEffect(() => {
        const loadAccounts = async () => {
            if (!user) return;
            try {
                const userData = await usersApi.getById(user.id);
                // Map the accounts from the user data
                const userAccounts: AccountDto[] = userData.accounts.map((a: any) => ({
                    id: a.id,
                    accountNumber: a.accountNumber,
                    type: a.type,
                    status: a.status,
                    balance: a.balance,
                    currency: a.currency,
                    createdAt: new Date().toISOString(),
                }));
                setAccounts(userAccounts);
            } catch (err) {
                console.error('Failed to load accounts:', err);
            } finally {
                setInitialLoading(false);
            }
        };
        loadAccounts();
    }, [user]);

    const handleCreateAccount = async () => {
        if (!user) return;

        setLoading(true);
        setError(null);
        try {
            const request: CreateAccountRequest = {
                userId: user.id,
                type: selectedType,
                currency: 'USD',
            };
            const newAccount = await accountsApi.create(request);
            setAccounts([...accounts, newAccount]);
            setShowCreateModal(false);
        } catch (err: any) {
            setError(err.response?.data?.error || 'Failed to create account');
        } finally {
            setLoading(false);
        }
    };

    if (initialLoading) {
        return (
            <div className="flex items-center justify-center h-64">
                <div className="animate-spin rounded-full h-12 w-12 border-4 border-blue-500 border-t-transparent"></div>
            </div>
        );
    }

    return (
        <div>
            {/* Header */}
            <div className="flex justify-between items-center mb-8">
                <div>
                    <h1 className="text-3xl font-bold text-gray-900">Dashboard</h1>
                    <p className="text-gray-600">Manage your accounts and transactions</p>
                </div>
                <button
                    onClick={() => setShowCreateModal(true)}
                    className="bg-blue-600 hover:bg-blue-700 text-white px-6 py-3 rounded-lg font-semibold shadow-lg hover:shadow-xl transition-all duration-200 flex items-center space-x-2"
                >
                    <span className="text-xl">+</span>
                    <span>New Account</span>
                </button>
            </div>

            {/* Accounts Grid */}
            {accounts.length === 0 ? (
                <div className="bg-white rounded-2xl shadow-lg p-12 text-center">
                    <div className="w-20 h-20 bg-blue-100 rounded-full flex items-center justify-center mx-auto mb-4">
                        <span className="text-4xl text-blue-600 font-bold">$</span>
                    </div>
                    <h2 className="text-xl font-semibold text-gray-900 mb-2">No Accounts Yet</h2>
                    <p className="text-gray-600 mb-6">Create your first bank account to get started</p>
                    <button
                        onClick={() => setShowCreateModal(true)}
                        className="bg-blue-600 hover:bg-blue-700 text-white px-6 py-3 rounded-lg font-semibold"
                    >
                        Create Account
                    </button>
                </div>
            ) : (
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                    {accounts.map((account) => (
                        <AccountCard key={account.id} account={account} />
                    ))}
                </div>
            )}

            {/* Create Account Modal */}
            {showCreateModal && (
                <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
                    <div className="bg-white rounded-2xl p-8 max-w-md w-full mx-4 shadow-2xl">
                        <h2 className="text-2xl font-bold text-gray-900 mb-6">Create New Account</h2>

                        <div className="space-y-4 mb-6">
                            {accountTypes.map((type) => (
                                <label
                                    key={type.value}
                                    className={`block p-4 border-2 rounded-xl cursor-pointer transition-all ${selectedType === type.value
                                        ? 'border-blue-500 bg-blue-50'
                                        : 'border-gray-200 hover:border-gray-300'
                                        }`}
                                >
                                    <input
                                        type="radio"
                                        name="accountType"
                                        value={type.value}
                                        checked={selectedType === type.value}
                                        onChange={() => setSelectedType(type.value)}
                                        className="sr-only"
                                    />
                                    <div className="flex justify-between items-center">
                                        <div>
                                            <p className="font-semibold text-gray-900">{type.label}</p>
                                            <p className="text-sm text-gray-500">{type.description}</p>
                                        </div>
                                        {selectedType === type.value && (
                                            <svg className="w-6 h-6 text-blue-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                                            </svg>
                                        )}
                                    </div>
                                </label>
                            ))}
                        </div>

                        {error && (
                            <div className="mb-4 p-3 bg-red-100 text-red-700 rounded-lg">
                                {error}
                            </div>
                        )}

                        <div className="flex space-x-4">
                            <button
                                onClick={() => setShowCreateModal(false)}
                                className="flex-1 px-4 py-3 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50"
                            >
                                Cancel
                            </button>
                            <button
                                onClick={handleCreateAccount}
                                disabled={loading}
                                className="flex-1 px-4 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50"
                            >
                                {loading ? 'Creating...' : 'Create Account'}
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}
