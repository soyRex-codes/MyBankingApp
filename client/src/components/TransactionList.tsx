import type { TransactionDto } from '../types';

interface TransactionListProps {
    transactions: TransactionDto[];
}

const typeIcons: Record<string, { icon: string; color: string }> = {
    Deposit: { icon: '↓', color: 'text-green-500 bg-green-100' },
    Withdrawal: { icon: '↑', color: 'text-red-500 bg-red-100' },
    TransferIn: { icon: '←', color: 'text-blue-500 bg-blue-100' },
    TransferOut: { icon: '→', color: 'text-orange-500 bg-orange-100' },
};

export default function TransactionList({ transactions }: TransactionListProps) {
    const formatCurrency = (amount: number, currency: string) => {
        return new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: currency,
        }).format(amount);
    };

    const formatDate = (dateString: string) => {
        return new Date(dateString).toLocaleDateString('en-US', {
            month: 'short',
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit',
        });
    };

    if (transactions.length === 0) {
        return (
            <div className="text-center py-12 text-gray-500">
                <p className="text-lg">No transactions yet</p>
                <p className="text-sm">Make a deposit to get started</p>
            </div>
        );
    }

    return (
        <div className="divide-y divide-gray-200">
            {transactions.map((transaction) => {
                const typeInfo = typeIcons[transaction.type] || typeIcons.Deposit;
                const isCredit = transaction.type === 'Deposit' || transaction.type === 'TransferIn';

                return (
                    <div key={transaction.id} className="py-4 flex items-center justify-between hover:bg-gray-50 px-2 rounded-lg transition-colors">
                        <div className="flex items-center space-x-4">
                            <div className={`w-10 h-10 rounded-full flex items-center justify-center ${typeInfo.color}`}>
                                <span className="text-lg font-bold">{typeInfo.icon}</span>
                            </div>
                            <div>
                                <p className="font-medium text-gray-900">{transaction.description}</p>
                                <p className="text-sm text-gray-500">{formatDate(transaction.createdAt)}</p>
                            </div>
                        </div>
                        <div className="text-right">
                            <p className={`font-semibold ${isCredit ? 'text-green-600' : 'text-red-600'}`}>
                                {isCredit ? '+' : '-'}{formatCurrency(transaction.amount, transaction.currency)}
                            </p>
                            <p className="text-xs text-gray-400 font-mono">{transaction.referenceNumber}</p>
                        </div>
                    </div>
                );
            })}
        </div>
    );
}
