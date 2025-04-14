export interface ResetPasswordDto {
  UserId: string;
  Token: string;
  NewPassword: string;
}
