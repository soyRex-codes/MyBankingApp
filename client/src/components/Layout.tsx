import { Outlet } from 'react-router-dom';
import Navbar from './Navbar';

export default function Layout() {
    return (
        <div className="min-h-screen bg-gray-100">
            <Navbar />
            <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
                <Outlet />
            </main>
            <footer className="bg-gray-800 text-gray-400 py-6 mt-auto">
                <div className="max-w-7xl mx-auto px-4 text-center">
                    <p>&copy; 2026 MyBankingApp. Portfolio Project.</p>
                </div>
            </footer>
        </div>
    );
}
