import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
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
    //// In a real client, we would do this when starting a new game, after the player picks a tile.
    //this.game$ = gameService.startNew({
    //  field: { width: 10, height: 10 },
    //  mines: 10,
    //  move: { x: Math.floor(Math.random() * 10), y: Math.floor(Math.random() * 10) }
    //}).pipe(first());
    //this.game$.subscribe(g => this.minefield = g.uncovered);
  }

  ngOnInit(): void {
    this.initialField$ = this.form.valueChanges.pipe(
      map(({ field }: GameCreation) => {
        var grid = ('.'.repeat(field.width) + '\r\n').repeat(field.height);
        return grid.substring(0, grid.length - 2);  // chop off the trailing \r\n
      }));

    this.form.setValue({
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

  public initialField$: Observable<string>;
  public isAuthenticated$: Observable<boolean>;
  public savedGames$: Observable<Game[]>;

  public playerMoved(position: Point) {
    // Start a new game and go there
    var creation: GameCreation = Object.assign({}, this.form.value, { move: position });
    this.gameService.startNew(creation).subscribe(game => this.router.navigate(['game', game.id]));
  }

  public form = this.fb.group({
    field: this.fb.group({
      width: [0, [Validators.required, Validators.min(1), Validators.max(32)]],
      height: [0, [Validators.required, Validators.min(1), Validators.max(32)]],
    }),
    mines: [0, [Validators.required, Validators.min(1)]]
  });
}
