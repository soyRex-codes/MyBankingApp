import { Link } from 'react-router-dom';
import type { AccountDto } from '../types';

interface AccountCardProps {
    account: AccountDto;
}

const accountTypeColors: Record<string, { bg: string; text: string; badge: string }> = {
    Checking: { bg: 'from-green-500 to-green-600', text: 'text-green-100', badge: 'bg-green-400' },
    Savings: { bg: 'from-blue-500 to-blue-600', text: 'text-blue-100', badge: 'bg-blue-400' },
    MoneyMarket: { bg: 'from-purple-500 to-purple-600', text: 'text-purple-100', badge: 'bg-purple-400' },
};

export default function AccountCard({ account }: AccountCardProps) {
    const colors = accountTypeColors[account.type] || accountTypeColors.Checking;

    const formatCurrency = (amount: number, currency: string) => {
        return new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: currency,
        }).format(amount);
    };

    return (
        <Link to={`/accounts/${account.id}`}>
            <div className={`bg-gradient-to-br ${colors.bg} rounded-2xl p-6 text-white shadow-xl hover:shadow-2xl transform hover:-translate-y-1 transition-all duration-300 cursor-pointer`}>
                <div className="flex justify-between items-start mb-4">
                    <div>
                        <span className={`${colors.badge} text-xs font-semibold px-2.5 py-0.5 rounded-full text-gray-900`}>
                            {account.type}
                        </span>
                    </div>
                    <div className={`w-12 h-8 bg-white/20 rounded-md flex items-center justify-center`}>
                        <span className="text-sm font-mono">****</span>
                    </div>
                </div>

                <div className="mb-4">
                    <p className="text-sm opacity-80">Account Number</p>
                    <p className="font-mono text-lg tracking-wider">
                        •••• •••• {account.accountNumber.slice(-4)}
                    </p>
                </div>

                <div>
                    <p className="text-sm opacity-80">Available Balance</p>
                    <p className="text-3xl font-bold">
                        {formatCurrency(account.balance, account.currency)}
                    </p>
                </div>

                <div className="mt-4 flex justify-between items-center">
                    <span className={`text-xs px-2 py-1 rounded ${account.status === 'Active' ? 'bg-green-400/30' : 'bg-red-400/30'}`}>
                        {account.status}
                    </span>
                    <span className="text-xs opacity-70">Click to view</span>
                </div>
            </div>
        </Link>
    );
}
