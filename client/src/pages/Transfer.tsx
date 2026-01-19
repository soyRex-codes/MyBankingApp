import { useState } from 'react';
import { Link } from 'react-router-dom';
import { transactionsApi } from '../services/api';

export default function Transfer() {
    const [sourceAccountId, setSourceAccountId] = useState('');
    const [destAccountId, setDestAccountId] = useState('');
    const [amount, setAmount] = useState('');
    const [description, setDescription] = useState('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState(false);

    const handleTransfer = async (e: React.FormEvent) => {
        e.preventDefault();

        if (!sourceAccountId || !destAccountId || !amount) {
            setError('Please fill in all required fields');
            return;
        }

        if (sourceAccountId === destAccountId) {
            setError('Source and destination accounts must be different');
            return;
        }

        setLoading(true);
        setError(null);

        try {
            await transactionsApi.transfer({
                sourceAccountId,
                destinationAccountId: destAccountId,
                amount: parseFloat(amount),
                description: description || undefined,
            });
            setSuccess(true);
            setAmount('');
            setDescription('');
        } catch (err: any) {
            setError(err.response?.data?.error || 'Transfer failed');
        } finally {
            setLoading(false);
        }
    };

    if (success) {
        return (
            <div className="max-w-md mx-auto">
                <div className="bg-white rounded-2xl shadow-lg p-8 text-center">
                    <div className="w-20 h-20 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-4">
                        <svg className="w-10 h-10 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                        </svg>
                    </div>
                    <h2 className="text-2xl font-bold text-gray-900 mb-2">Transfer Complete!</h2>
                    <p className="text-gray-600 mb-6">Your funds have been transferred successfully.</p>
                    <div className="flex space-x-4">
                        <button
                            onClick={() => setSuccess(false)}
                            className="flex-1 px-4 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
                        >
                            New Transfer
                        </button>
                        <Link
                            to="/"
                            className="flex-1 px-4 py-3 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 text-center"
                        >
                            Dashboard
                        </Link>
                    </div>
                </div>
            </div>
        );
    }

    return (
        <div className="max-w-md mx-auto">
            <h1 className="text-3xl font-bold text-gray-900 mb-2">Transfer Funds</h1>
            <p className="text-gray-600 mb-8">Move money between your accounts</p>

            <div className="bg-white rounded-2xl shadow-lg p-8">
                <form onSubmit={handleTransfer} className="space-y-6">
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">
                            From Account ID
                        </label>
                        <input
                            type="text"
                            value={sourceAccountId}
                            onChange={(e) => setSourceAccountId(e.target.value)}
                            className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                            placeholder="Source account GUID"
                        />
                    </div>

                    <div className="flex justify-center">
                        <div className="w-10 h-10 bg-blue-100 rounded-full flex items-center justify-center text-blue-600">
                            ↓
                        </div>
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">
                            To Account ID
                        </label>
                        <input
                            type="text"
                            value={destAccountId}
                            onChange={(e) => setDestAccountId(e.target.value)}
                            className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                            placeholder="Destination account GUID"
                        />
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">
                            Amount
                        </label>
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
                        <label className="block text-sm font-medium text-gray-700 mb-1">
                            Description (optional)
                        </label>
                        <input
                            type="text"
                            value={description}
                            onChange={(e) => setDescription(e.target.value)}
                            className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                            placeholder="What's this transfer for?"
                        />
                    </div>

                    {error && (
                        <div className="p-3 bg-red-100 text-red-700 rounded-lg">
                            {error}
                        </div>
                    )}

                    <button
                        type="submit"
                        disabled={loading}
                        className="w-full py-4 bg-blue-600 hover:bg-blue-700 text-white rounded-lg font-semibold disabled:opacity-50 transition-colors"
                    >
                        {loading ? 'Processing...' : 'Transfer Funds'}
                    </button>
                </form>
            </div>
        </div>
    );
}
