import { Injectable } from '@angular/core';
import { ServiceBase } from '../../base.service';
import { environment } from '../../../../environments/environment';
import { ApplicationUser } from '../../../models/management/applicationUser';

@Injectable({
  providedIn: 'root'
})
export class UserService extends ServiceBase<ApplicationUser> {
  endpoint = 'user';
  origin = environment.identityControlApi;
}
