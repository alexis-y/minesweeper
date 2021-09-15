import { GameResult } from './game-result';
import { Size } from './size';

export interface Game {
  id: string;
  field: Size;
  uncovered: string;
  result: GameResult | null;
};
