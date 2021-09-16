import { Injectable } from '@angular/core';

import { Observable } from 'rxjs';

import { DataService } from './data.service';
import { Game, Size, Point } from './model';

export type GameCreation = {
  field: Size,
  mines: number;
  move: Point
};

@Injectable()
export class GameDataService extends DataService {

  protected collectionSegment: string = 'api/games';

  public get(id: string): Observable<Game> {
    return this._get(id);
  }

  public getAll(): Observable<Game[]> {
    return this._get('');
  }

  public startNew(creation: GameCreation): Observable<Game> {
    return this._post('', creation);
  }

  public move(game: string | Game, position: Point): Observable<Game> {
    if (!(typeof game == 'string')) game = game.id;
    return this._post(`${game}/move`, position);
  }

  public redFlag(game: string | Game, position: Point): Observable<Game> {
    if (!(typeof game == 'string')) game = game.id;
    return this._post(`${game}/flag/red-flag`, position);
  }

  public questionFlag(game: string | Game, position: Point): Observable<Game> {
    if (!(typeof game == 'string')) game = game.id;
    return this._post(`${game}/flag/question`, position);
  }

  public clearFlag(game: string | Game, position: Point): Observable<Game> {
    if (!(typeof game == 'string')) game = game.id;
    return this._post(`${game}/flag/clear`, position);
  }
}
