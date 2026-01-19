import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useUser } from '../context/UserContext';

export default function Navbar() {
    const location = useLocation();
    const navigate = useNavigate();
    const { user, logout } = useUser();

    const isActive = (path: string) => location.pathname === path;

    const handleLogout = () => {
        logout();
        navigate('/login');
    };

    return (
        <nav className="bg-gradient-to-r from-blue-900 to-blue-700 shadow-lg">
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div className="flex items-center justify-between h-16">
                    <div className="flex items-center">
                        <Link to="/" className="flex items-center space-x-2">
                            <div className="w-10 h-10 bg-white rounded-full flex items-center justify-center">
                                <span className="text-blue-900 font-bold text-xl">B</span>
                            </div>
                            <span className="text-white font-bold text-xl">MyBank</span>
                        </Link>
                    </div>
                    <div className="flex items-center space-x-4">
                        <Link
                            to="/"
                            className={`px-4 py-2 rounded-lg transition-all duration-200 ${isActive('/')
                                ? 'bg-white text-blue-900 font-semibold'
                                : 'text-white hover:bg-blue-800'
                                }`}
                        >
                            Dashboard
                        </Link>
                        <Link
                            to="/transfer"
                            className={`px-4 py-2 rounded-lg transition-all duration-200 ${isActive('/transfer')
                                ? 'bg-white text-blue-900 font-semibold'
                                : 'text-white hover:bg-blue-800'
                                }`}
                        >
                            Transfer
                        </Link>
                        {user && (
                            <>
                                <span className="text-white/80 text-sm">
                                    Hi, {user.firstName}
                                </span>
                                <button
                                    onClick={handleLogout}
                                    className="px-4 py-2 rounded-lg text-white hover:bg-red-600 transition-all duration-200"
                                >
                                    Logout
                                </button>
                            </>
                        )}
                    </div>
                </div>
            </div>
        </nav>
    );
}
