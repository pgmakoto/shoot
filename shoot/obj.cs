using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Forms;
using System.Numerics;
using static System.Math;


namespace shoot
{
    public class man
    {
        public float Width;
        public float Height;

        public PointF target = new PointF();
        public Timer tim = new Timer();
        public List<obj> objs = new List<obj>();
        public List<msl> msls = new List<msl>();
        public List<explosion> exps = new List<explosion>();
        public fighter F_target;

        Random rand = new Random();

        public man()
        {

        }

        public void Ignite(PointF pos, float vx, float vy)
        {
            if (msls.Count <20)
            {
                msl o = new msl(new Quaternion(pos.X, pos.Y, 0, 0), new Quaternion(vx, vy, 0, 0));
                msls.Add(o);
            }
        }
        int r = 0;
        public void Add(PointF pos, float vx, float vy)
        {
            float rx = rand.Next(100) / 5 -10;
            float ry = rand.Next(100) / 5 -10 ;
            if (objs.Count < 2000)//&& r==0)
            {
                obj o = new obj(new Quaternion(pos.X+rx/10, pos.Y+ry/10, 0, 0), new Quaternion(vx+rx, vy+ry, 0, 0));
                objs.Add(o);
                r = 1;
            }
            else r = 0;

        }
        public void detonation(PointF pos, float vx, float vy)
        {
            if (exps.Count < 100)//&& r==0)
            {
                explosion o = new explosion(new Quaternion(pos.X, pos.Y, 0, 0), new Quaternion(vx/2, vy/2, 0, 0));
                exps.Add(o);

                float x = 0;
                float y = 0;
                obj smkc = new obj(new Quaternion(pos.X + x, pos.Y + y, 0, 0), new Quaternion(vx / 2 + x * 3, vy / 2 + y * 3, 0, 0));
                smkc.size =10;
                objs.Add(smkc);
                x = 15;
                for (int i = 0; i <12; i++)
                {
                    obj smk = new obj(new Quaternion(pos.X +x, pos.Y +y, 0, 0), new Quaternion(vx/2+x*3, vy/2+y*3, 0, 0)) ;
                    smk.size = 30;
                    objs.Add(smk);

                    x = x  + y * 0.5f;
                    y = y  - x * 0.5f;

                }

                r = 1;
            }
            else r = 0;

        }

        public void refresh(float dt = 0.1f)
        {
            //force
            for (int i = 0; i < objs.Count; i++)
            {
                //煙同士
                for (int j = i + 1; j < objs.Count; j++)
                {
                    Quaternion d = objs[i].pos - objs[j].pos;

                    if (Abs(d.X) + Abs(d.Y) < 60)
                    {
                        float l = (Abs(d.X) + Abs(d.Y))/2;
                        objs[i].vel.X += dt * d.X / l;
                        objs[i].vel.Y += dt * d.Y / l;

                        objs[j].vel.X += dt * -d.X / l;
                        objs[j].vel.Y += dt * -d.Y / l;
                    }
                }
                //地表との処理
                if (Height - objs[i].pos.Y < 100)
                {
                    float dg = (float)Height - objs[i].pos.Y;
                    if (Abs(dg) > 50f)
                    {
                        objs[i].vel.Y += -50f / dg;
                    }
                    else if(Abs(dg) > 40f)
                    {
                        objs[i].vel.X = objs[i].vel.X + objs[i].vel.Y * 0.5f;
                        objs[i].vel.Y *= 0.5f;
                    }
                    else
                    {
                        objs[i].vel.X = objs[i].vel.X - objs[i].vel.Y * 0.5f;
                        objs[i].vel.Y *= 0.5f;
                    }
                }

                //   float brk = (float)Sqrt(objs[i].vel.X * objs[i].vel.X + objs[i].vel.Y * objs[i].vel.Y);


            }
            //Clip
            for (int i = objs.Count - 1; i >= 0; i--)
            {
                int r = rand.Next(100);
                if (objs[i].size > 80  && r>95)
                {
                    objs.RemoveAt(i);
                }
                else if (objs[i].size > 150 || objs[i].pos.X < 0 || objs[i].pos.X > Width || objs[i].pos.Y < 0 || objs[i].pos.Y > Height-40f)
                {
                    objs.RemoveAt(i);
                }

            } 
            
            int k = 0;
            while ( k < msls.Count)
            {
                //tama同士
                int j = msls.Count - 1;
                while ( j > k)
                {
                    Quaternion dif = msls[k].pos - msls[j].pos;
                    Quaternion difv = msls[k].vel - msls[j].vel;
                    if (Abs(dif.X ) + Abs(dif.Y) < 50)//dif.X * dif.X + dif.Y * dif.Y < 100 )
                    {
                        if (Abs(difv.X) +Abs( difv.Y)  > 1000)//(difv.X * difv.X + difv.Y * difv.Y > 1000000)
                        {
                            detonation(new PointF(msls[k].pos.X, msls[k].pos.Y)
                              , msls[k].vel.X + msls[j].vel.X, msls[k].vel.Y + msls[j].vel.Y);

                            msls.RemoveAt(j);
                            //      msls.RemoveAt(k);
                            j--;// j--;
                                //      k++;
                            continue;
                        }
                        else
                        {
             //              msls[k].vel.X -= difv.X/5;
             //              msls[k].vel.Y -= difv.Y/5;
             //              msls[j].vel.X += difv.X/5;
             //              msls[j].vel.Y += difv.Y/5;
                        }
                    }
                    j--;
                }
                k++;
            }

            //爆風　x　ミサイル
            for (int i = 0; i < exps.Count; i++)
            {
                for (int j = msls.Count-1; j > i; j--)
                {
                    Quaternion dif = msls[j].pos - exps[i].pos;
                    if (dif.X * dif.X + dif.Y * dif.Y < exps[i].size * exps[i].size/2)
                    {
                        detonation(new PointF(msls[j].pos.X, msls[j].pos.Y)
                            ,  msls[j].vel.X/2,  msls[j].vel.Y/2);
                        msls.RemoveAt(j);
                    }
                }
            }


            //////////////////////////////////////////////////////////////
            Quaternion tgt = new Quaternion(F_target.pos.X, F_target.pos.Y, 0, 0);
            for (int i = 0; i < msls.Count; i++)
            {
                if (msls[i].trace(tgt))
                {
                    Add(new PointF(msls[i].pos.X - 10 * msls[i].dir.X, msls[i].pos.Y - 10 * msls[i].dir.Y)
                        , - 200 * msls[i].dir.X// msls[i].vel.X * 0.5f
                        ,  - 200 * msls[i].dir.Y);//msls[i].vel.Y * 0.5f
                }
            }

            ///////////////////////////////////////////////////////////
            //move
            for (int i = 0; i < objs.Count; i++)
            {
                objs[i].move(dt);
            }
            //move
            for (int i = 0; i < msls.Count; i++)
            {
                msls[i].move(dt);
            }
            for (int i = 0; i < exps.Count; i++)
            {
                exps[i].move(dt);
            }
            if (F_target != null)
            {
                F_target.pilot(new Quaternion(target.X, target.Y, 0, 0));
                F_target.move(dt);
                Add(new PointF(F_target.pos.X - 10 * F_target.dir.X, F_target.pos.Y - 10 * F_target.dir.Y)
                    , -200 * F_target.dir.X// msls[i].vel.X * 0.5f
                    , -200 * F_target.dir.Y);//msls[i].vel.Y * 0.5f

            }
            if (F_target.pos.X > Width) F_target.pos.X -= Width;
            if (F_target.pos.X <0) F_target.pos.X += Width;
            //Clip
            for (int i = msls.Count - 1; i >= 0; i--)
            {
                    Quaternion dif = msls[i].pos - new Quaternion(F_target.pos.X, F_target.pos.Y,0,0);
                if (dif.X * dif.X + dif.Y * dif.Y < 100)
                {
                    detonation(new PointF(msls[i].pos.X, msls[i].pos.Y)
                        , msls[i].vel.X / 2, msls[i].vel.Y / 2);
                    msls.RemoveAt(i);
                }
                else
                if (msls[i].pos.Y > Height - 40)
                {
                    detonation(new PointF(msls[i].pos.X, Height - 40), 0, 0);
                    msls.RemoveAt(i);

                }
            }
            for (int i = exps.Count - 1; i >= 0; i--)
            {
                if (exps[i].life == 0)
                {
                    exps.RemoveAt(i);
                }
            }

        }

        public void Draw(Graphics g)
        {
            for (int i = 0; i < objs.Count; i++)
            {
                //          float r = objs[i].size;
                int sz = (int)(150 / objs[i].size)*3;
                if (sz > 255) sz = 255;
                if (sz > 0)
                {
                    Brush br = new SolidBrush(Color.FromArgb(sz, Color.LightGray));
                    g.FillEllipse(br, objs[i].pos.X - objs[i].size / 2
                        , objs[i].pos.Y - objs[i].size / 2, objs[i].size, objs[i].size);
                }
            }
            Pen pen = new Pen(Color.Gray, 3);
            Pen penR = new Pen(Color.OrangeRed, 3);

            for (int i = 0; i < msls.Count; i++)
            {
                g.DrawLine(penR
                    , msls[i].pos.X, msls[i].pos.Y
                    , msls[i].pos.X + msls[i].dir.X * 5, msls[i].pos.Y + msls[i].dir.Y * 5);

                g.DrawLine(pen, msls[i].pos.X, msls[i].pos.Y
                    , msls[i].pos.X - msls[i].dir.X * 10, msls[i].pos.Y - msls[i].dir.Y *10);
            }

            Brush brfra = new SolidBrush(Color.FromArgb(150, Color.White));
            Brush brexp = new SolidBrush(Color.FromArgb(80, Color.LightGoldenrodYellow));
            for (int i = 0; i < exps.Count; i++)
            {

                if (exps[i].life > 9)
                {
                    g.FillEllipse(brfra, exps[i].pos.X - 80
                    , exps[i].pos.Y - 80, 160,160);
                }
                else
                {
                    g.FillEllipse(brexp, exps[i].pos.X - exps[i].size / 2
                        , exps[i].pos.Y - exps[i].size / 2, exps[i].size, exps[i].size);
                }
            }
            if (F_target != null)
            {
                g.FillEllipse(brfra, F_target.pos.X - F_target.size / 2
                    , F_target.pos.Y - F_target.size / 2, F_target.size, F_target.size);
                g.FillEllipse(brfra, F_target.pos.X - F_target.size / 3 + F_target.dir.X * 5
                    , F_target.pos.Y - F_target.size / 3 + F_target.dir.Y * 5, F_target.size * 0.66f , F_target.size * 0.66f ); ;
            }

        }

    }

    public class obj
    {
        public Quaternion pos;
        public Quaternion vel;
        public float size = 2f;
        public obj(Quaternion p, Quaternion v)
        {

            pos = p;
            vel = v;
        }

        public virtual void move(float dt)
        {
            pos += vel * dt;
            pos.W = 0;
            size++;
            vel.X *= 0.97f;
            vel.Y *= 0.97f;

        }

    }
    public class fighter : obj
    {
        public Quaternion dir = new Quaternion(0, -1, 0, 0);

        public fighter(Quaternion p, Quaternion v) : base(p, v)
        {
            size = 15;
            vel = new Quaternion(1, 0, 0, 0);
    }
    public override void move(float dt)
        {
            pos += vel * dt;
            pos.W = 0;
            vel.X *= 0.97f;
            vel.Y *= 0.97f;

        }


        public void pilot(Quaternion target)
        {
            Quaternion targetDiff = target - pos;// - vel * 0.1f;

            float targetl = (float)Sqrt(targetDiff.X * targetDiff.X + targetDiff.Y * targetDiff.Y);

            targetDiff.X /= targetl;
            targetDiff.Y /= targetl;

            if (targetl > 50) targetl = 50;

            float tx, ty;
            tx = targetDiff.X * dir.Y - targetDiff.Y * dir.X;
            ty = targetDiff.X * dir.X + targetDiff.Y * dir.Y;



            dir.X = dir.X + dir.Y * tx/50;
            dir.Y = dir.Y - dir.X * tx/50;

            float dirl = 1f / (float)Sqrt(dir.X * dir.X + dir.Y * dir.Y);
            dir.X = targetDiff.X * dirl;
            dir.Y = targetDiff.Y * dirl;

            if (ty < 0) ty = 0;
            //加速
            vel.X += targetDiff.X * 20 * targetl/50;
            vel.Y += targetDiff.Y * 20 * targetl/50;

        }

    }

    public class explosion : obj
    {
        public Quaternion pos;
        public Quaternion vel;
        public int life = 10;
        public explosion(Quaternion p, Quaternion v) : base(p, v)
        {
            pos = p;
            vel = v;
            size = 10f;
        }

        public override void move(float dt)
        {
            pos += vel * dt;
            pos.W = 0;
            if (life-- > 0)
            {
                size = (size + 100)/2;
            }

        }
    }


    public class msl : obj
    {
        public float rot = 0;
        public int fuel = 200;
        public Quaternion dir = new Quaternion(0,-1,0,0);

        public msl(Quaternion p, Quaternion v) : base(p, v)
        {
            size = 5;
            fuel = 200;
            vel = new Quaternion(0,0, 0, 0);
        }
        public override void move(float dt)
        {
            pos += vel * dt;
            pos.W = 0;
            vel.X *= 0.98f;
            vel.Y *= 0.98f;

        }
        public bool trace(Quaternion target)
        {
            bool res = false;
            Quaternion targetDiff = target - pos-vel*0.1f;

            targetDiff.Y -= 4;
            //            Quaternion dir = targetDiff * vel;
            if (fuel > 0)
            {

                float targetl = (float)Sqrt(targetDiff.X * targetDiff.X + targetDiff.Y * targetDiff.Y);
                float tx, ty;
                tx = targetDiff.X * dir.Y - targetDiff.Y * dir.X;
                ty = targetDiff.X * dir.X + targetDiff.Y * dir.Y;
           //     float nsk = targetDiff.X * dir.X + targetDiff.Y * dir.Y;
                if (fuel < 200)
                {
                    if (tx < 0)
                    {
                        rot = (rot - 0.15f) * 0.5f;
                    }
                    else
                    {
                        rot = (rot + 0.15f) * 0.5f;
                    }
                    dir.X = dir.X + dir.Y * rot;
                    dir.Y = dir.Y - dir.X * rot;

                    float rots = -rot * 0.2f;
                    float rotc = (float)Sqrt(1f - rot * rot * 0.1f);


                    float x = vel.X * rotc - vel.Y * rots;
                    float y = vel.X * rots + vel.Y * rotc;
                    vel.X = x;
                    vel.Y = y;


                    float dirl = 1f / (float)Sqrt(dir.X * dir.X + dir.Y * dir.Y);
                    dir.X *= dirl;
                    dir.Y *= dirl;
                }
                if (ty > 0 || fuel>200)
                {
                    //加速
                    vel.X += dir.X * 15 * 400 / (200+fuel);
                    vel.Y += dir.Y * 15 * 400 / (200 + fuel);
                    fuel--;
                    res = true;
                }

                //    dir = new Quaternion(targetDiff.X / targetl, targetDiff.Y / targetl, 0, 0);
            }
            else
            {
                dir.X = dir.X + dir.Y * rot;
                dir.Y = dir.Y - dir.X * rot;

            }
            vel.Y += 2f;

            float v2 = (float)Sqrt(vel.X * vel.X + vel.Y * vel.Y);

            //減速
            return res;
        }


    }
}
