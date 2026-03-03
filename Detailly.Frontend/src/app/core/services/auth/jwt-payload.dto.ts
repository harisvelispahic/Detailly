export interface JwtPayloadDto {
  sub?: string;

  // these two are the ones you actually get
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'?: string;
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'?: string;

  // your custom flag claims
  is_admin?: string;
  is_manager?: string;
  is_employee?: string;
  is_fleet?: string;

  ver?: string;
  iat?: number;
  exp?: number;
  aud?: string | string[];
  iss?: string;
}
