import {BaseIdentityModel} from '../baseIdentityModel';

export interface ApiScope extends BaseIdentityModel {
  name: string;
  displayName: string;
  description: string;
}
