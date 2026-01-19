// API Types matching backend DTOs

export interface AccountDto {
    id: string;
    accountNumber: string;
    type: string;
    status: string;
    balance: number;
    currency: string;
    createdAt: string;
}

export interface TransactionDto {
    id: string;
    referenceNumber: string;
    type: string;
    status: string;
    amount: number;
    currency: string;
    balanceAfter: number;
    description: string;
    createdAt: string;
}

export interface PagedResult<T> {
    items: T[];
    totalCount: number;
    pageNumber: number;
    pageSize: number;
    totalPages: number;
    hasNextPage: boolean;
    hasPreviousPage: boolean;
}

export interface CreateAccountRequest {
    userId: string;
    type: number; // 1=Checking, 2=Savings, 3=MoneyMarket
    currency?: string;
}

export interface DepositRequest {
    amount: number;
    currency?: string;
    description?: string;
}

export interface WithdrawRequest {
    amount: number;
    currency?: string;
    description?: string;
}

export interface TransferRequest {
    sourceAccountId: string;
    destinationAccountId: string;
    amount: number;
    currency?: string;
    description?: string;
}

export interface ApiError {
    error: string;
    errorCode: string;
}
