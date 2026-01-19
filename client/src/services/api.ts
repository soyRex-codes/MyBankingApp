import axios from 'axios';
import type {
    AccountDto,
    TransactionDto,
    PagedResult,
    CreateAccountRequest,
    DepositRequest,
    WithdrawRequest,
    TransferRequest
} from '../types';

const API_BASE_URL = 'http://localhost:5114/api';

const api = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

// Account endpoints
export const accountsApi = {
    getById: async (id: string): Promise<AccountDto> => {
        const response = await api.get<AccountDto>(`/accounts/${id}`);
        return response.data;
    },

    create: async (request: CreateAccountRequest): Promise<AccountDto> => {
        const response = await api.post<AccountDto>('/accounts', request);
        return response.data;
    },

    deposit: async (accountId: string, request: DepositRequest): Promise<TransactionDto> => {
        const response = await api.post<TransactionDto>(`/accounts/${accountId}/deposit`, request);
        return response.data;
    },

    withdraw: async (accountId: string, request: WithdrawRequest): Promise<TransactionDto> => {
        const response = await api.post<TransactionDto>(`/accounts/${accountId}/withdraw`, request);
        return response.data;
    },

    getTransactions: async (
        accountId: string,
        pageNumber = 1,
        pageSize = 10
    ): Promise<PagedResult<TransactionDto>> => {
        const response = await api.get<PagedResult<TransactionDto>>(
            `/accounts/${accountId}/transactions`,
            { params: { pageNumber, pageSize } }
        );
        return response.data;
    },
};

// Transaction endpoints
export const transactionsApi = {
    transfer: async (request: TransferRequest): Promise<{ outgoingTransaction: TransactionDto; incomingTransaction: TransactionDto }> => {
        const response = await api.post('/transactions/transfer', request);
        return response.data;
    },
};

// User endpoints
export interface UserDto {
    id: string;
    email: string;
    firstName: string;
    lastName: string;
}

export interface CreateUserRequest {
    email: string;
    firstName: string;
    lastName: string;
}

export const usersApi = {
    create: async (request: CreateUserRequest): Promise<UserDto> => {
        const response = await api.post<UserDto>('/users', request);
        return response.data;
    },

    getByEmail: async (email: string): Promise<UserDto> => {
        const response = await api.get<UserDto>(`/users/by-email/${encodeURIComponent(email)}`);
        return response.data;
    },

    getById: async (id: string): Promise<UserDto & { accounts: any[] }> => {
        const response = await api.get(`/users/${id}`);
        return response.data;
    },
};

export default api;
