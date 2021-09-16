import { Injectable } from '@angular/core';

import { Observable } from 'rxjs';

import { DataService } from './data.service';
import { Game, Size, Point } from './model';

type GameCreation = {
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

  public move(id: string, position: Point): Observable<Game> {
    return this._post(`${id}/move`, position);
  }

  public redFlag(id: string, position: Point): Observable<Game> {
    return this._post(`${id}/flag/red-flag`, position);
  }

  public question(id: string, position: Point): Observable<Game> {
    return this._post(`${id}/flag/question`, position);
  }

  public clearFlag(id: string, position: Point): Observable<Game> {
    return this._post(`${id}/flag/clear`, position);
  }
}
