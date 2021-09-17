import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import { Observable } from 'rxjs';
import { filter, map, switchMap } from 'rxjs/operators';
import { AuthorizeService } from '../../api-authorization/authorize.service';

import { GameCreation, GameDataService } from '../game-data.service';
import { Game, Point } from '../model';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  constructor(private gameService: GameDataService, private router: Router, private fb: FormBuilder, private authorizeService: AuthorizeService) {
  }

  ngOnInit(): void {
    this.form.reset({
      field: { width: 10, height: 10 },
      mines: 8
    });

    // For resuming old games
    this.isAuthenticated$ = this.authorizeService.isAuthenticated();
    this.savedGames$ = this.isAuthenticated$.pipe(
      filter(value => value),
      switchMap(() => this.gameService.getAll())
    );
  }

  public isAuthenticated$: Observable<boolean>;
  public savedGames$: Observable<Game[]>;

  public playerMoved(position: Point) {
    // Start a new game and go there
    var creation: GameCreation = Object.assign({}, this.form.value, { move: position });
    this.gameService.startNew(creation).subscribe(game => this.router.navigate(['game', game.id]));
  }

  readonly form = this.fb.group({
    field: this.fb.group({
      width: [0, [Validators.required, Validators.min(1), Validators.max(32)]],
      height: [0, [Validators.required, Validators.min(1), Validators.max(32)]],
    }),
    mines: [0, [Validators.required, Validators.min(1), CustomValidators.dynamicMax(() => this.form ? (<number>this.form.get('field.width').value) * (<number>this.form.get('field.height').value) - 1 : 100)]]  // must be lower than the number of cells
  });

  // The weird "this.form ? [use form] : [do not use form]" above is because the validator runs
  // the first time before this.form is assigned

  readonly initialField$ = this.form.valueChanges.pipe(
    //filter(() => this.form.valid),
    map(({ field }: GameCreation) => {
      var grid = ('.'.repeat(field.width) + '\r\n').repeat(field.height);
      return grid.substring(0, grid.length - 2);  // chop off the trailing \r\n
    }));

}

class CustomValidators {
  static dynamicMax(valueFn: () => number) {
    return function dynamicMaxValidator(control: AbstractControl) {
      if (control.value !== undefined && (Number.isNaN(control.value) || control.value > valueFn())) {
        return { 'dynamicMax': true };
      }
      return null;
    }
  }
}
