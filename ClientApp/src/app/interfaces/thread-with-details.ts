import {User} from "./user";

export interface ThreadWithDetails {
  id: string,
  topic: string,
  closed: boolean,
  creationDate: Date,
  author?: User
}
