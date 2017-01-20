// Boris Chan
// May 16th, 2014
// This is a game like Zelda where player kill the dragon.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Assignment_3
{
    public partial class Form1 : Form
    {
        // Game  
        bool isGameRunning = false;
        bool timeToUpdate = false;

        // Store time info
        int previousTime;
        int currentTime;
        int timePassed;
        const int FRAME_RATE = 10;
       
        // Map Location
        const int MAP_X = 0;
        const int MAP_y = 0;
        const int MAP_LENGTH = 960;
        const int MAP_WIDTH = 672;
        Rectangle mapBoundingBox;

        // Player Life
        int playerLife = 10;
        int playerDamageCoolDown;

        // Player Location
        bool anyArrowKeysPress = false;
        int characterX = 460;
        int characterY = 550;
        int characterPreviousX;
        int characterPreviousY;
        const int CHARACTER_SIDE_LENGTH = 31;
        
        Rectangle characterBoundingBox;
        Image currentPlayerImage;

        // Player Movement
        bool moveDown = false;
        bool moveUp = false;
        bool moveLeft = false;
        bool moveRight = false;
        const int CHARACTER_SPEED = 10;

        // Player side
        bool front = false;
        bool back = false;
        bool left = false;
        bool right = false;

        // Player Sword Movement
        int swordCoolDown = 0;
        bool swordSwing = false;
        int swordX;
        int swordY;
        const int SWORD_LENGTH = 25;
        const int SWORD_WIDTH = 11;

        Rectangle swordUpDownBoundingBox;
        Rectangle swordLeftRightBoundingBox;
        Image currentSwordimage;

        // Dragon Life
        int dragonLife = 20;
        bool dragonDamaged = false;
        int damageCoolDown = 10;

        // Dragon Body Part Location and Size
        // Dragon Body
        const int DRAGON_BODY_X = 460;
        const int DRAGON_BODY_Y = 0;
        const int DRAGON_BODY_WIDTH = 150;
        const int DRAGON_BODY_LENGTH = 231;
        Rectangle dragonBodyBoundingBox;
        
        // Dragon Neck
        const int DRAGON_NECK_WIDTH = 63;
        const int DRAGON_NECK_LENGTH = 75;

        int dragonNeck1X = DRAGON_BODY_X + 47;
        int dragonNeck1Y = DRAGON_BODY_Y + 182;
        Rectangle dragonNeck1BoundingBox;
                
        int dragonNeck2X = DRAGON_BODY_X + 47;
        int dragonNeck2Y = DRAGON_BODY_Y + 208;
        Rectangle dragonNeck2BoundingBox;

        // Dragon Head
        int dragonHeadX = DRAGON_BODY_X + 47;
        int dragonHeadY = DRAGON_BODY_Y + 216;
        const int DRAGON_HEAD_WIDTH = 63;
        const int DRAGON_HEAD_LENGTH = 105;
        Rectangle dragonHeadBoundingBox;        
        
        // Fireball variables               
        bool drawFireball = false;
        const int FIREBALL_SPEED = 10;
        int fireballX;
        int fireballY;
        const int FIREBALL_LENGTH = 56;
        const int FIREBALL_WIDTH = 30;
        Rectangle fireballBoundingBox;

        // Fireball slope calculations
        bool fireballCalculate;
        int run;
        int rise;
        double slopeX;
        double slopeY;
        int speedX;
        int speedY;

        // Calculate if Fireball out of bounds
        bool fireballActive = true;

        // Count Down
        const int LIFE_LOST = 1;
        const int COOL_DOWN = 1;

        // Obstacle Bounding boxes
        Rectangle wellBoundingBox;
        Rectangle sheep1BoundingBox;
        Rectangle sheep2BoundingBox;
        Rectangle sheep3BoundingBox;
        Rectangle sheep4BoundingBox;
        Rectangle house1BoundingBox;
        Rectangle house2BoundingBox;

        //Font
        Font titleFont = new Font("Garamond", 24.0f);
        Font dragonLifeFont = new Font("Garamond", 20.0f);
        Font characterLifeFont = new Font("Garamond", 10.0f);

        public Form1()
        {
            InitializeComponent();
                        
            // System time
            currentTime = Environment.TickCount;
            previousTime = currentTime;
            timePassed = currentTime - previousTime;

            // Initialize  Map Bounding Box
            mapBoundingBox = new Rectangle(MAP_X, MAP_y, MAP_LENGTH, MAP_WIDTH);

            // Initialize Character Bounding Boxes 
            characterBoundingBox = new Rectangle(characterX, characterY, CHARACTER_SIDE_LENGTH, CHARACTER_SIDE_LENGTH);

            swordUpDownBoundingBox = new Rectangle(swordX, swordY, SWORD_WIDTH, SWORD_LENGTH);
            swordLeftRightBoundingBox = new Rectangle(swordX, swordY, SWORD_LENGTH, SWORD_WIDTH);

            // Initialize Dragon Parts Bounding Boxes
            dragonBodyBoundingBox = new Rectangle(DRAGON_BODY_X, DRAGON_BODY_Y, DRAGON_BODY_WIDTH, DRAGON_BODY_LENGTH);
            dragonNeck1BoundingBox = new Rectangle(dragonNeck1X, dragonNeck1Y, DRAGON_NECK_WIDTH, DRAGON_NECK_LENGTH);
            dragonNeck2BoundingBox = new Rectangle(dragonNeck2X, dragonNeck2Y, DRAGON_NECK_WIDTH, DRAGON_NECK_LENGTH);
            dragonHeadBoundingBox = new Rectangle(dragonHeadX, dragonHeadY, DRAGON_HEAD_WIDTH, DRAGON_HEAD_LENGTH);

            // Initialize FireBall
            fireballX = dragonHeadX + 21;
            fireballY = dragonHeadY + 85;
            fireballBoundingBox = new Rectangle(fireballX, fireballY, FIREBALL_WIDTH, FIREBALL_LENGTH);
            drawFireball = true;

            // Fireball slope calculation
            run = characterX - fireballX;
            rise = characterY - fireballY;
            slopeX = Math.Sqrt(Math.Pow(run, 2) / (Math.Pow(run, 2) + Math.Pow(rise, 2)));
            slopeY = Math.Sqrt(Math.Pow(rise, 2) / (Math.Pow(run, 2) + Math.Pow(rise, 2)));
            speedX = (int)(FIREBALL_SPEED * (double)slopeX);
            speedY = (int)(FIREBALL_SPEED * (double)slopeY);

            if (run < 0)
            {
                speedX = -speedX;
            }

            if (rise < 0)
            {
                speedY = -speedY;
            }

            // Initialize Obstacle bounding box
            wellBoundingBox = new Rectangle(680, 302, 22, 61);
            sheep1BoundingBox = new Rectangle(581, 391, 17, 14);
            sheep2BoundingBox = new Rectangle(645, 491, 25, 19);
            sheep3BoundingBox = new Rectangle(745, 522, 16, 15);
            sheep4BoundingBox = new Rectangle(515, 551, 25, 19);
            house1BoundingBox = new Rectangle(323, 300, 90, 160);
            house2BoundingBox = new Rectangle(703, 20, 165, 180);
        }
        
        /// <summary>
        /// Update the game with a loop
        /// </summary>
        void CustomTimer()
        {
            do
            {
                // Determine wherther to draw the next frame
                currentTime = Environment.TickCount;
                timePassed = currentTime - previousTime;

                if (timePassed >= FRAME_RATE)
                {
                    previousTime = currentTime;
                    timeToUpdate = true;


                    // Player Movement
                    PlayerMove();
                    PlayerAttack();

                    // Dragon Movement
                    DragonMove();
                    MoveFireball();
                                       


                    // Collision
                    Collision();
                    FireballCollision();

                    // Graphics
                    Refresh();
                }

                Application.DoEvents();
            } while (isGameRunning == true);

        }

        /// <summary>
        /// Draw Graphic
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (isGameRunning == true)
            {
                if (timeToUpdate == true)
                {
                    timeToUpdate = false;

                    // Map Graphic
                    e.Graphics.DrawImage(Properties.Resources.Farm_Map, mapBoundingBox);

                    // Showing Dragon life
                    e.Graphics.DrawString(dragonLife + "/20", dragonLifeFont, Brushes.Black, DRAGON_BODY_X, DRAGON_BODY_Y);

                    // Showing Character life
                    e.Graphics.DrawString(playerLife + "/10", characterLifeFont, Brushes.Black, characterX, characterY - 15);

                    // Character Graphic Mak
                    if (anyArrowKeysPress == false)
                    {
                        e.Graphics.DrawImage(Properties.Resources.Front, characterBoundingBox);
                    }
                    else
                    {
                        e.Graphics.DrawImage(currentPlayerImage, characterBoundingBox);
                    }

                    // Sword Graphic
                    if (swordCoolDown >= 5 && swordSwing == true)
                    {
                        if (front || back)
                        {
                            e.Graphics.DrawImage(currentSwordimage, swordUpDownBoundingBox);
                        }
                        else if (left || right)
                        {
                            e.Graphics.DrawImage(currentSwordimage, swordLeftRightBoundingBox);
                        }
                    }

                    // Dragon Graphic
                    if (dragonDamaged == true)
                    {
                        // Damaged Dragon Graphic
                        e.Graphics.DrawImage(Properties.Resources.DragonHurtBody, dragonBodyBoundingBox);
                        e.Graphics.DrawImage(Properties.Resources.DragonHurtNeck, dragonNeck1BoundingBox);
                        e.Graphics.DrawImage(Properties.Resources.DragonHurtNeck, dragonNeck2BoundingBox);
                        e.Graphics.DrawImage(Properties.Resources.DragonHurtHead, dragonHeadBoundingBox);
                    }
                    else
                    {
                        // Normal Dragon Graphic
                        e.Graphics.DrawImage(Properties.Resources.DragonBody, dragonBodyBoundingBox);
                        e.Graphics.DrawImage(Properties.Resources.DragonNeck, dragonNeck1BoundingBox);
                        e.Graphics.DrawImage(Properties.Resources.DragonNeck, dragonNeck2BoundingBox);
                        e.Graphics.DrawImage(Properties.Resources.DragonHead, dragonHeadBoundingBox);
                    }

                    // Fireball Graphic
                    if (drawFireball == true)
                    {
                       e.Graphics.DrawImage(Properties.Resources.Fireball_Down, fireballBoundingBox);
                       MoveFireball();
                    }


                }
            }

            else
            {
                e.Graphics.DrawImage(Properties.Resources.Farm_Map, mapBoundingBox);
                e.Graphics.DrawString("Game Paused: Press P to continue.", titleFont, Brushes.LavenderBlush, this.ClientSize.Width / 2 - 180, this.ClientSize.Height / 2);
            }


        }

        /// <summary>
        /// Move character when player presses arrow keys. 
        /// Also check for obticles and out of screen.
        /// </summary>
        void PlayerMove()
        {
            // Store character previous XY
            characterPreviousX = characterX;
            characterPreviousY = characterY;

            if (moveDown)
            {
                // Check if Player out of screen
                if (characterY + CHARACTER_SPEED > ClientSize.Height - characterBoundingBox.Height)
                {
                    moveDown = false;
                }
                else
                {
                    // Move character down
                    characterY += CHARACTER_SPEED;
                    characterBoundingBox.Location = new Point(characterX, characterY);

                    // Check for obstacles
                    Obstacles();
                }
                
                // Character Graphic
                currentPlayerImage = Properties.Resources.Front;
            }

            else if (moveUp)
            {
                // Check if Player out of screen
                if (characterY - CHARACTER_SPEED < 0)
                {
                    moveUp = false;
                }
                else
                {
                    // Move character up
                    characterY -= CHARACTER_SPEED;
                    characterBoundingBox.Location = new Point(characterX, characterY);

                    // Check for obstacles
                    Obstacles();
                }

                // Character Graphic
                currentPlayerImage = Properties.Resources.Back;
            }

            else if (moveLeft)
            {
                // Check if Player out of screen
                if (characterX - CHARACTER_SPEED < 0)
                {
                    moveLeft = false;
                }
                else
                {
                    // Move character left
                    characterX -= CHARACTER_SPEED;
                    characterBoundingBox.Location = new Point(characterX, characterY);

                    // Check for obstacles
                    Obstacles();
                }

                // Character Graphic
                currentPlayerImage = Properties.Resources.Left;
            }

            else if (moveRight)
            {
                // Check if Player out of screen
                if (characterX + CHARACTER_SPEED > ClientSize.Width - characterBoundingBox.Width)
                {
                    moveRight = false;
                }
                else
                {
                    // Move character right
                    characterX += CHARACTER_SPEED;
                    characterBoundingBox.Location = new Point(characterX, characterY);

                    // Check for obstacles
                    Obstacles();
                }

                // Character Graphic
                currentPlayerImage = Properties.Resources.Right;
            }


        }

        /// <summary>
        /// Determine sword location and side 
        /// Calculate sword cool down to see when character can attack again
        /// </summary>
        void PlayerAttack()
        {
            // State player side and sword side
            if (front)
            {
                swordX = characterX + 15;
                swordY = characterY + 31;
                swordUpDownBoundingBox.Location = new Point(swordX, swordY);
                currentSwordimage = Properties.Resources.SwordDown;
            }
            else if (back)
            {
                swordX = characterX + 15;
                swordY = characterY - 25;
                swordUpDownBoundingBox.Location = new Point(swordX, swordY);
                currentSwordimage = Properties.Resources.SwordUp;

            }
            else if (left)
            {
                swordX = characterX - 21;
                swordY = characterY + 15;
                swordLeftRightBoundingBox.Location = new Point(swordX, swordY);
                currentSwordimage = Properties.Resources.SwordLeft;
            }
            else if (right)
            {
                swordX = characterX + 27;
                swordY = characterY + 15;
                swordLeftRightBoundingBox.Location = new Point(swordX, swordY);
                currentSwordimage = Properties.Resources.SwordRight;
            }

            // Count down for cool down
            if (swordCoolDown > 0)
            {
                swordCoolDown -= COOL_DOWN;

                if (swordCoolDown == 0)
                {
                    swordSwing = false;
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                moveDown = true;
                anyArrowKeysPress = true;

                // Player side
                front = true;
                back = false;
                left = false;
                right = false;
            }
            else if (e.KeyCode == Keys.Up)
            {
                moveUp = true;
                anyArrowKeysPress = true;

                // Player side
                front = false;
                back = true;
                left = false;
                right = false;
            }
            else if (e.KeyCode == Keys.Left)
            {
                moveLeft = true;
                anyArrowKeysPress = true;

                // Player side
                front = false;
                back = false;
                left = true;
                right = false;
            }
            else if (e.KeyCode == Keys.Right)
            {
                moveRight = true;
                anyArrowKeysPress = true;

                // Player side
                front = false;
                back = false;
                left = false;
                right = true;
            }
            else if (e.KeyCode == Keys.Space)
            {
                // Attack
                if (swordCoolDown == 0 && swordSwing == false)
                {
                    swordSwing = true;
                    swordCoolDown = 10;
                }
            }
            else if (e.KeyCode == Keys.P)
            {                
                // If game is not running, start game
                if (isGameRunning == false)
                {
                    playerDamageCoolDown = 25;

                    // Run Loop
                    isGameRunning = true;
                    CustomTimer();                    
                }

                // If game is running, stop game
                else if (isGameRunning == true)
                {
                    isGameRunning = false;
                    Refresh();
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                isGameRunning = false;
                Application.Exit();
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                moveDown = false;
                currentPlayerImage = Properties.Resources.Front;
            }
            else if (e.KeyCode == Keys.Up)
            {
                moveUp = false;
                currentPlayerImage = Properties.Resources.Back;
            }
            else if (e.KeyCode == Keys.Left)
            {
                moveLeft = false;
                currentPlayerImage = Properties.Resources.Left;
            }
            else if (e.KeyCode == Keys.Right)
            {
                moveRight = false;
                currentPlayerImage = Properties.Resources.Right;
            }
        }

        /// <summary>
        /// Move Dragon Head and Neck
        /// </summary>
        void DragonMove()
        {
            // Dragon head movement
            dragonHeadX = (DRAGON_BODY_X + 47 + characterX) / 2;

            // Set limit on Dragon head 
            if (dragonHeadX <= 468)
            {
                dragonHeadX = 468;
            }
            else if (dragonHeadX >= 540)
            {
                dragonHeadX = 540;
            }

            // Dragon neck movement
            dragonNeck2X = (DRAGON_BODY_X + 47 + dragonHeadX) / 2;
            dragonNeck1X = (DRAGON_BODY_X + 47 + dragonNeck2X) / 2;

            // Update location
            dragonHeadBoundingBox.Location = new Point(dragonHeadX, dragonHeadY);
            dragonNeck1BoundingBox.Location = new Point(dragonNeck1X, dragonNeck1Y);
            dragonNeck2BoundingBox.Location = new Point(dragonNeck2X, dragonNeck2Y);
        }

        /// <summary>
        /// Draw the first fireball
        /// </summary>
        void DragonAttack()
        {
            drawFireball = true;
        }           

        /// <summary>
        /// Calculate fireball slope and speed
        /// Move the fireball and to see if the fireball goes out of bounds
        /// </summary>
        void MoveFireball()
        {
            if (fireballActive)
            {
                if (fireballCalculate)
                {
                    fireballCalculate = false;

                    // Fireball slope calculation
                    run = characterX - fireballX;
                    rise = characterY - fireballY;
                    slopeX = Math.Sqrt(Math.Pow(run, 2) / (Math.Pow(run, 2) + Math.Pow(rise, 2)));
                    slopeY = Math.Sqrt(Math.Pow(rise, 2) / (Math.Pow(run, 2) + Math.Pow(rise, 2)));
                    speedX = (int)(FIREBALL_SPEED * (double)slopeX);
                    speedY = (int)(FIREBALL_SPEED * (double)slopeY);
                    
                    // Switch direction
                    if (run < 0)
                    {
                        speedX = -speedX;
                    }

                    if (rise < 0)
                    {
                        speedY = -speedY;
                    }
                }

               
                // Move Fireball
                fireballX += speedX;
                fireballY += speedY;
                fireballBoundingBox = new Rectangle(fireballX, fireballY, 18, 19);
                
                

                // check if fireball is out of bounds
                if (fireballY >= this.ClientSize.Height)
                {
                    drawFireball = false;
                    RespawnFireBall();
                }
                else if (fireballY + fireballBoundingBox.Height <= 0)
                {
                    drawFireball = false;
                    RespawnFireBall();
                }
                else if (fireballX >= this.ClientSize.Width)
                {
                    drawFireball = false;
                    RespawnFireBall();
                }
                else if (fireballX + fireballBoundingBox.Width <= 0)
                {
                    drawFireball = false;
                    RespawnFireBall();
                }
            }

        }
        
        /// <summary>
        /// Relocate and Draw fireball after previous fireball is out of bound
        /// </summary>
        void RespawnFireBall()
        {          
            // Re-initialize FireBall
            fireballX = dragonHeadX + 21;
            fireballY = dragonHeadY + 85;
            fireballBoundingBox.Location = new Point(fireballX, fireballY) ;
            
            // Draw new fireball and calculate slope
            drawFireball = true;
            fireballCalculate = true;
        }

        /// <summary>
        /// Make player can't walk through certain locations in the map
        /// </summary>
        void Obstacles()
        {
            // Character cannot go through dragon body
            if (characterBoundingBox.IntersectsWith(dragonBodyBoundingBox))
            {
                characterBoundingBox.Location = new Point(characterPreviousX, characterPreviousY);
                characterX = characterPreviousX;
                characterY = characterPreviousY;
            }
            // Character cannot go through the well
            else if (characterBoundingBox.IntersectsWith(wellBoundingBox))
            {
                characterBoundingBox.Location = new Point(characterPreviousX, characterPreviousY);
                characterX = characterPreviousX;
                characterY = characterPreviousY;
            }
            // Character cannot go through sheeps
            else if (characterBoundingBox.IntersectsWith(sheep1BoundingBox))
            {
                characterBoundingBox.Location = new Point(characterPreviousX, characterPreviousY);
                characterX = characterPreviousX;
                characterY = characterPreviousY;
            }
            else if (characterBoundingBox.IntersectsWith(sheep2BoundingBox))
            {
                characterBoundingBox.Location = new Point(characterPreviousX, characterPreviousY);
                characterX = characterPreviousX;
                characterY = characterPreviousY;
            }
            else if (characterBoundingBox.IntersectsWith(sheep3BoundingBox))
            {
                characterBoundingBox.Location = new Point(characterPreviousX, characterPreviousY);
                characterX = characterPreviousX;
                characterY = characterPreviousY;
            }
            else if (characterBoundingBox.IntersectsWith(sheep4BoundingBox))
            {
                characterBoundingBox.Location = new Point(characterPreviousX, characterPreviousY);
                characterX = characterPreviousX;
                characterY = characterPreviousY;
            }
            // Character cannot go through houses
            else if (characterBoundingBox.IntersectsWith(house1BoundingBox))
            {
                characterBoundingBox.Location = new Point(characterPreviousX, characterPreviousY);
                characterX = characterPreviousX;
                characterY = characterPreviousY;
            }
            else if (characterBoundingBox.IntersectsWith(house2BoundingBox))
            {
                characterBoundingBox.Location = new Point(characterPreviousX, characterPreviousY);
                characterX = characterPreviousX;
                characterY = characterPreviousY;
            }
        }
        
        /// <summary> 
        /// If the sword hit the dragon, deal damage to dragon's health
        /// </summary>
        void Collision()
        {
            // Detect if sword cut the dragon
            if (swordSwing == true && dragonDamaged == false)
            {
                if (swordUpDownBoundingBox.IntersectsWith(dragonBodyBoundingBox) || swordLeftRightBoundingBox.IntersectsWith(dragonBodyBoundingBox))
                {
                    dragonLife = dragonLife - LIFE_LOST;
                    dragonDamaged = true;
                    damageCoolDown = 10;
                }
                else if (swordUpDownBoundingBox.IntersectsWith(dragonNeck1BoundingBox) || swordLeftRightBoundingBox.IntersectsWith(dragonNeck1BoundingBox))
                {
                    dragonLife = dragonLife - LIFE_LOST;
                    dragonDamaged = true;
                    damageCoolDown = 10;
                }
                else if (swordUpDownBoundingBox.IntersectsWith(dragonNeck2BoundingBox) || swordLeftRightBoundingBox.IntersectsWith(dragonNeck2BoundingBox))
                {
                    dragonLife = dragonLife - LIFE_LOST;
                    dragonDamaged = true;
                    damageCoolDown = 10;
                }
                else if (swordUpDownBoundingBox.IntersectsWith(dragonHeadBoundingBox) || swordLeftRightBoundingBox.IntersectsWith(dragonHeadBoundingBox))
                {
                    dragonLife = dragonLife - LIFE_LOST;
                    dragonDamaged = true;
                    damageCoolDown = 10;
                }
            }

            // Red Damage turn to normal 
            if (damageCoolDown > 0)
            {
                damageCoolDown -= COOL_DOWN;

                if (damageCoolDown == 0)
                {
                    dragonDamaged = false;
                }
            }

            // Dragon killed
            if (dragonLife == 0)
            {
                isGameRunning = false;

                if (MessageBox.Show("You WIN, Do you want to restart?", "Restart", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Application.Restart();
                    characterX = 460;
                    characterY = 550;
                    characterBoundingBox = new Rectangle(characterX, characterY, 29, 31);
                }
                else
                {
                    Application.Exit();
                }

            }
        }

        /// <summary>
        /// If the fireball hits the character, deal damage to character's health
        /// </summary>
        void FireballCollision()
        {
            // Determine if character is recently hit
            if (playerDamageCoolDown > 0)
            {
                playerDamageCoolDown -= 1;
            }

            // If fireball hit the character
            if (fireballBoundingBox.IntersectsWith(characterBoundingBox))
            {
                if (playerDamageCoolDown == 0)
                {
                    drawFireball = false;
                    playerLife -= LIFE_LOST;
                    playerDamageCoolDown = 6;
                }
            }

            // Character dies
            if (playerLife == 0)
            {
                isGameRunning = false;

                if (MessageBox.Show("You LOST, Do you want to restart?", "Restart", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Application.Restart();
                }
                else
                {
                    Application.Exit();
                }

            }
        }
    }
}
