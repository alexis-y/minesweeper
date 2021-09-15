import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';

/**
 * Base data access service.
 * */
@Injectable()
export abstract class DataService {

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

  /**
   * Gets the URL segment(s) to the collection that this service handles.
   * */
  protected abstract readonly collectionSegment: string;

  protected _get<T>(relativeUri: string): Observable<T> {
    return this.http.get<T>(this.buildUrl(relativeUri));
  }

  protected _post<T>(relativeUri: string, body?: any): Observable<T> {
    return this.http.post<T>(this.buildUrl(relativeUri), body);
  }


  protected buildUrl(...segments: string[]): string {
    return DataService.join(this.baseUrl, this.collectionSegment, ...segments);
  }

  private static join(...segments: string[]): string {
    return segments
      .filter(segment => !!segment)
      .reduce((url, segment) => {
        if (segment.startsWith('/')) throw new Error(`Illegal segment, can not start with '/'.`);  // We don't do "root" paths
        if (!url.endsWith('/')) url += '/';
        return url + segment;
      });
  }
}
