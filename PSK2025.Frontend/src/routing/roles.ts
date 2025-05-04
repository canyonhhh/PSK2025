export enum Role {
  MANAGER = "Manager",
  BARISTA = "Barista",
  CUSTOMER = "Customer",
}

export const roleToRoleEnum: Record<string, Role> = Object.values(Role).reduce<
  Record<string, Role>
>((acc, value) => {
  acc[value] = value;

  return acc;
}, {});
