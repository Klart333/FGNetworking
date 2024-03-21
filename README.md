Overall score: 6

(I kinda misunderstood the game at first, I did not know you could move. I thought the game was a competition to shoot the most mines, which is what I have done. Don't ask how I never noticed that the ships move, I really don't know... But also I think the game makes more sense this way anyway, without making Player Death, Respawns or Health Packs etc.)

12. Homing Missiles
- Press space to shoot a green tur- I mean homing missile towards the nearest (directionally) player. The missile is a networkObject and on hit it notifies the server which tells all the instances of that player to become red for an amount of time, if it's the owner it also disables input and decreases the NetworkVariable Score by one. Scripts: Missile, PlayerController, FiringAction

13. Live scoreboard
- Mostly worked with the bullets (SingleBullet), mines (StandardMine) and of course the leaderboard ui I made (UILeaderboard & UIPlayerScore). Made the bullets into NetworkObjects so I know which player shot and incremented a network variable on the correct player, that's pretty much it. 

Also there's player names now :)

Thanks for your time!
