export interface UserWithDetails {
  id: string,
  username: string,
  email: string,
  name?: string,
  registrationDate: string,
  roles: string[]
}
