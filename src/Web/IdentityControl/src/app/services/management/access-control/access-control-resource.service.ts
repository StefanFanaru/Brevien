import { Injectable } from '@angular/core';
import { ServiceBase } from '../../base.service';
import { environment } from '../../../../environments/environment';
import { AccessControlResource } from '../../../models/management/resource';

@Injectable({
  providedIn: 'root'
})
export class AccessControlResourceService extends ServiceBase<AccessControlResource> {
  endpoint = 'resource';
  origin = environment.identityControlApi;
}
