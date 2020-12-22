import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import 'src/app/helpers/stringExtensions';
import { ActivatedRoute, Router } from '@angular/router';
import { IdentityServerBaseService } from '../../../services/management/identity-server/identity-server-base-service';
import { ApiResource } from '../../../models/management/apiResource';
import { IdentityServerApiResourceService } from '../../../services/management/identity-server/identity-server-api-resource.service';
import { SearchService } from '../../../services/search.service';
import { SortDirection } from '../../../models/sortDirection';
import { ApiScope } from '../../../models/management/apiScope';

enum ApiResourceFilter {
  Enabled,
  Disabled
}

@Component({
  selector: 'app-api-resources',
  templateUrl: './api-resources.component.html',
  styleUrls: ['../identity-server.scss', './api-resources.component.scss']
})
export class ApiResourcesComponent
  extends IdentityServerBaseService<ApiResource,
    IdentityServerApiResourceService>
  implements OnInit {
  displayedColumns: (keyof ApiResource)[] = [
    'selected',
    'displayName',
    'description',
    'name',
    'created',
    'updated',
    'enabled'
  ];
  editForm: FormGroup;
  rows: ApiResource[];
  submitted = false;
  apiScopeEditForm: FormGroup;
  addMode: boolean;
  pageTitle = 'API Resources';

  constructor(
    public httpService: IdentityServerApiResourceService,
    public formBuilder: FormBuilder,
    public searchService: SearchService,
    public route: ActivatedRoute,
    public router: Router
  ) {
    super();
    this.queryParams.sortColumn = 'DisplayName';
    this.queryParams.sortDirection = SortDirection.Asc;
  }

  get itemSelected() {
    return this.selectedRows[0];
  }

  set itemSelected(value) {
    this.selectedRows[0] = value;
  }

  ngOnInit(): void {
    this.initialize();
  }

  buildForm(item: ApiScope = null) {
    this.isFormVisible = true;
    this.addMode = !item;

    this.editForm = this.formBuilder.group({
      name: [this.addMode ? '' : item.name, Validators.required],
      displayName: [this.addMode ? '' : item.displayName, Validators.required],
      description: [this.addMode ? '' : item.description, Validators.required]
    });
    this.convertDisplayName();
  }

  onFilterSelect(value) {
    let filter = null;
    if (value !== 'All') {
      filter =
        value === 'Enabled'
          ? ApiResourceFilter.Enabled
          : ApiResourceFilter.Disabled;
    }
    super.getFilteredData(filter);
  }
}
