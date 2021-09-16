import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { GameDataService } from '../game-data.service';
import { Game, Point } from '../model';

type GameResolveData = {
  game: Game
}

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
})
export class GameComponent implements OnInit {
  constructor(private route: ActivatedRoute, private data: GameDataService) {
    
  }

  ngOnInit(): void {
    this.route.data.subscribe(({ game }: GameResolveData) => {
      this.game = game;
    });
  }

  public game: Game;

  public move(position: Point) {
    this.data.move(this.game, position).subscribe(g => this.game = g);
  }
  public placeRedFlag(position: Point) {
    this.data.redFlag(this.game, position).subscribe(g => this.game = g);
  }
  public placeQuestionFlag(position: Point) {
    this.data.questionFlag(this.game, position).subscribe(g => this.game = g);
  }
  public clearFlag(position: Point) {
    this.data.clearFlag(this.game, position).subscribe(g => this.game = g);
  }
}
