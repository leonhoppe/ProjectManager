import {Component, ElementRef, ViewChild} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {CrudService} from "../../services/crud.service";

@Component({
  selector: 'app-project',
  templateUrl: './project.component.html',
  styleUrls: ['./project.component.scss']
})
export class ProjectComponent {
  @ViewChild('frame') frame: ElementRef;

  public constructor(public route: ActivatedRoute, public crud: CrudService) {
    setTimeout(this.getRoute.bind(this), 0);
  }

  public getRoute() {
    this.route.params.subscribe(params => {
      this.frame.nativeElement.src = this.crud.backendUrl + 'projects/' + params['id'] + '/url?token=' + this.crud.authKey;
    })
  }

}
