import {User} from "./user";

export interface PostWithDetails {
  id: string,
  content: string,
  publishDate: string,
  threadId: string,
  author?: User
}
