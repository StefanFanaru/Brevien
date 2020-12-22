import {Injectable} from '@angular/core';
import {ServiceBase} from '../../base.service';
import {environment} from '../../../../environments/environment';
import {ApiScope} from '../../../models/management/apiScope';
import {Client} from '../../../models/management/client';

@Injectable({
  providedIn: 'root'
})
export class IdentityServerClientService extends ServiceBase<Client> {
  endpoint = 'client';
  origin = environment.identityControlApi;
}
