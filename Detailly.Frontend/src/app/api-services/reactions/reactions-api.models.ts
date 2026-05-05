export enum ReactionType {
  None = 0,
  Like = 1,
  Dislike = 2,
}

export interface ReactionSummaryDto {
  likeCount: number;
  dislikeCount: number;
  myReaction: ReactionType | null;
}

export interface MyReactionDto {
  servicePackageId: number;
  reactionType: ReactionType;
}

export interface UpsertReactionCommand {
  reactionType: ReactionType;
}
