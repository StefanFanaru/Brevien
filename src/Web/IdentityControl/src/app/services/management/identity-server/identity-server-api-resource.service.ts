import {Injectable} from '@angular/core';
import {ServiceBase} from '../../base.service';
import {environment} from '../../../../environments/environment';
import {ApiScope} from '../../../models/management/apiScope';
import {ApiResource} from '../../../models/management/apiResource';

@Injectable({
  providedIn: 'root'
})
export class IdentityServerApiResourceService extends ServiceBase<ApiResource> {
  endpoint = 'api-resource';
  origin = environment.identityControlApi;
}
