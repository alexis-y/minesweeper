import { GameResult } from './game-result';
import { Size } from './size';

export interface Game {
  id: string;
  field: Size;
  mines: number;
  fieldState: string;
  result: GameResult | null;
};
