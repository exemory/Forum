import {User} from "./user";

export interface PostWithDetails {
  id: string,
  content: string,
  publishDate: Date,
  threadId: string,
  author?: User
}
