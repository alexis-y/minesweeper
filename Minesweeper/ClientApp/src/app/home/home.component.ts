import { Component } from '@angular/core';

import { Observable } from 'rxjs';
import { first } from 'rxjs/operators';

import { GameDataService } from '../game-data.service';
import { Game } from '../model';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  constructor(gameService: GameDataService) {
    // In a real client, we would do this when starting a new game, after the player picks a tile.
    this.game$ = gameService.startNew({
      field: { width: 10, height: 10 },
      mines: 10,
      move: { x: Math.floor(Math.random() * 10), y: Math.floor(Math.random() * 10) }
    }).pipe(first());
    this.game$.subscribe(g => this.minefield = g.uncovered);
  }

  public readonly game$: Observable<Game>
  //public get minefield$() { return this.game$.pipe(map(g => g.uncovered)) };  // TODO: Using this (plus async pipe) causes the request to repeat ad infinitum
  public minefield: string;
}
