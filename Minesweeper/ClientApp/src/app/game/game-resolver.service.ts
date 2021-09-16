import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, Router, RouterStateSnapshot } from '@angular/router';

import { EMPTY, Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { StatusCodes } from 'http-status-codes';

import { GameDataService } from '../game-data.service';
import { Game } from '../model';

@Injectable({
  providedIn: 'root'
})
export class GameResolverService implements Resolve<Game> {
  constructor(private data: GameDataService, private router: Router) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<Game> | Observable<never> {
    return this.data.get(route.paramMap.get('id')).pipe(
      catchError(error => {
        // ng router will swallow any exception in the resolver, so we have to deal with it here.
        if (error instanceof HttpErrorResponse && error.status === StatusCodes.GONE) {
          this.router.navigate(['/']);
          return EMPTY;
        }
        return throwError(error);
      })
    );
  }

}
