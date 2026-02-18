export interface AuthUser {
    userId: string;
    userName: string;
    email: string;
    nickName: string;
    systemName: string;
    serialNumber: string;
    roles: string[] | null;
}
