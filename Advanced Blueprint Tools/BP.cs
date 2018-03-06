﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Windows;
using System.IO;

namespace Advanced_Blueprint_Tools
{
    public class BP
    {
        public string blueprintpath { get; set; } 

        public dynamic blueprint { get; set; }

        public dynamic description { get; set; }

        public string icon { get; set; }

        public List<string> useduuids { get; set; } = new List<string>();

        public dynamic blocksxyz = new JObject();

        public int minx = 10000,maxx = -10000, miny = 10000, maxy = -10000, minz = 10000, maxz = -10000;

        public override string ToString()
        {
            return Convert.ToString(blueprint);
            
        }
        public void setblueprint(dynamic bp)
        {
            this.blueprint = bp;

            missingmod = false;
            //if(false)
            {
                useduuids.Clear();
                blocksxyz = new JObject();
                foreach (dynamic body in blueprint.bodies)
                    foreach (dynamic child in body.childs)
                    {

                        if (!useduuids.Contains(child.shapeId.ToString()))
                            useduuids.Add(child.shapeId.ToString());

                        dynamic correctedchild = getposandbounds(child);//outputs corrected child (default rotation, correct position)

                        string x = correctedchild.pos.x.ToString();
                        string y = correctedchild.pos.y.ToString();
                        string z = correctedchild.pos.z.ToString();
                        if (child.bounds == null) child.bounds = correctedchild.bounds;
                        if (blocksxyz[x] == null) blocksxyz[x] = new JObject();
                        if (blocksxyz[x][y] == null) blocksxyz[x][y] = new JObject();
                        if (blocksxyz[x][y][z] == null) blocksxyz[x][y][z] = new JObject();
                        if (blocksxyz[x][y][z].blocks == null) blocksxyz[x][y][z].blocks = new JArray();
                        blocksxyz[x][y][z].blocks.Add(child);

                        //get whole creation bounds:
                        if (correctedchild.pos.x < minx) minx = correctedchild.pos.x;
                        if (correctedchild.pos.x > maxx) maxx = correctedchild.pos.x;
                        if (correctedchild.pos.y < miny) miny = correctedchild.pos.y;
                        if (correctedchild.pos.y > maxy) maxy = correctedchild.pos.y;
                        if (correctedchild.pos.z < minz) minz = correctedchild.pos.z;
                        if (correctedchild.pos.z > maxz) maxz = correctedchild.pos.z;
                    }
                if (blueprint.joints != null)
                    foreach (dynamic joint in blueprint.joints)
                    {
                        if (!useduuids.Contains(joint.shapeId.ToString()))
                            useduuids.Add(joint.shapeId.ToString());
                    }
            }
            if (missingmod) MessageBox.Show("Missing mod for this blueprint! \nPlease download the required mod!\n\nwill work for now tho wiring/moving blocks not recommended!");
        }

        public BP(string blueprintpath, dynamic blueprint, dynamic description)
        {
            this.blueprintpath = blueprintpath;
            this.blueprint = blueprint;
            this.description = description;
        }

        public BP(string blueprintpath)
        {
            //load directory of blueprints
            this.blueprintpath = blueprintpath;
            try
            {
                this.icon = blueprintpath + @"\icon.png";
                this.blueprint = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(System.IO.File.ReadAllText(blueprintpath + @"\blueprint.json"));
                this.description = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(System.IO.File.ReadAllText(blueprintpath + @"\description.json"));

                missingmod = false;
                //if(false)
                {
                    for (int i = 0; i < blueprint.bodies.Count; i++)
                        for (int j = 0; j < blueprint.bodies[i].childs.Count ; j++)
                        {
                            if(!useduuids.Contains(blueprint.bodies[i].childs[j].shapeId.ToString()))
                                useduuids.Add(blueprint.bodies[i].childs[j].shapeId.ToString());

                            dynamic correctedchild = getposandbounds(blueprint.bodies[i].childs[j]);//outputs corrected child (default rotation, correct position)

                            string x = correctedchild.pos.x.ToString();
                            string y = correctedchild.pos.y.ToString();
                            string z = correctedchild.pos.z.ToString();
                            if (blueprint.bodies[i].childs[j].bounds == null) blueprint.bodies[i].childs[j].bounds = correctedchild.bounds;
                            if (blocksxyz[x] == null) blocksxyz[x] = new JObject();
                            if (blocksxyz[x][y] == null) blocksxyz[x][y] = new JObject();
                            if (blocksxyz[x][y][z] == null) blocksxyz[x][y][z] = new JObject();
                            if (blocksxyz[x][y][z].blocks == null) blocksxyz[x][y][z].blocks = new JArray();

                            blocksxyz[x][y][z].blocks.Add(this.blueprint.bodies[i].childs[j]);

                            //get whole creation bounds:
                            if (correctedchild.pos.x < minx) minx = correctedchild.pos.x;
                            if (correctedchild.pos.x > maxx) maxx = correctedchild.pos.x;
                            if (correctedchild.pos.y < miny) miny = correctedchild.pos.y;
                            if (correctedchild.pos.y > maxy) maxy = correctedchild.pos.y;
                            if (correctedchild.pos.z < minz) minz = correctedchild.pos.z;
                            if (correctedchild.pos.z > maxz) maxz = correctedchild.pos.z;
                        }
                    if(blueprint.joints != null)
                        for (int i = 0; i < blueprint.joints.Count; i++)
                        {
                            if (!useduuids.Contains(blueprint.joints[i].shapeId.ToString()))
                                useduuids.Add(blueprint.joints[i].shapeId.ToString());
                            blueprint.joints[i].xaxis = blueprint.joints[i].xaxisA;
                            blueprint.joints[i].zaxis = blueprint.joints[i].zaxisA;
                            blueprint.joints[i].pos = blueprint.joints[i].posA;
                            dynamic correctedchild = getposandbounds(blueprint.joints[i]);//outputs corrected child (default rotation, correct position)
                            correctedchild.pos = blueprint.joints[i].posA;
                            string x = correctedchild.pos.x.ToString();
                            string y = correctedchild.pos.y.ToString();
                            string z = correctedchild.pos.z.ToString();
                            if (blueprint.joints[i].bounds == null) blueprint.joints[i].bounds = correctedchild.bounds;
                            if (blocksxyz[x] == null) blocksxyz[x] = new JObject();
                            if (blocksxyz[x][y] == null) blocksxyz[x][y] = new JObject();
                            if (blocksxyz[x][y][z] == null) blocksxyz[x][y][z] = new JObject();
                            if (blocksxyz[x][y][z].blocks == null) blocksxyz[x][y][z].blocks = new JArray();

                            blocksxyz[x][y][z].blocks.Add(this.blueprint.joints[i]);
                        }
                }
                if(missingmod) MessageBox.Show("Missing mod for this blueprint! \nPlease download the required mod!\n\nwill work for now tho wiring/moving blocks not recommended!");

            }
            catch (Exception e)
            {
                this.blueprint = null;
                  MessageBox.Show(e.Message);
            }
        }


        public void Save()
        {

            string blueprinttext = Convert.ToString(blueprint);
            string descriptiontext = Newtonsoft.Json.JsonConvert.SerializeObject(description);
            
            if(!Directory.Exists(blueprintpath))
                System.IO.Directory.CreateDirectory(blueprintpath);

            System.IO.File.WriteAllText(blueprintpath + "\\blueprint.json", blueprinttext); //save blueprint
            System.IO.File.WriteAllText(blueprintpath + "\\description.json", descriptiontext); //save description

        }
        public void SaveAs(string name)
        {
            string newbp = System.IO.Directory.GetParent(blueprintpath) + @"\" + name;
            System.IO.Directory.CreateDirectory(newbp);

            string blueprinttext = Convert.ToString(blueprint);
            string descriptiontext = Newtonsoft.Json.JsonConvert.SerializeObject(description);

            System.IO.File.WriteAllText(newbp + "\\blueprint.json", blueprinttext); //save blueprint
            System.IO.File.WriteAllText(newbp + "\\description.json", descriptiontext); //save description
           if(icon!=null) System.IO.File.Copy(icon, newbp + @"\icon.png");


        }



        //get bounds from 
        private dynamic getbounds(dynamic part)
        {
            dynamic bounds = new Newtonsoft.Json.Linq.JObject();
            if (part.box != null)
            {
                return part.box;
            }
            if (part.hull != null)
            {
                bounds.x = part.hull.x;
                bounds.y = part.hull.y;
                bounds.z = part.hull.z;
                return bounds;
            }
            if (part.cylinder != null)
            {
                if (part.cylinder.axis.ToString().ToLower() == "x")
                {
                    bounds.x = part.cylinder.depth;
                    bounds.y = part.cylinder.diameter;
                    bounds.z = part.cylinder.diameter;
                    return bounds;

                }
                else
                if (part.cylinder.axis.ToString().ToLower() == "y")
                {
                    bounds.x = part.cylinder.diameter;
                    bounds.y = part.cylinder.depth;
                    bounds.z = part.cylinder.diameter;
                    return bounds;

                }
                else
                if (part.cylinder.axis.ToString().ToLower() == "z")
                {
                    bounds.x = part.cylinder.diameter;
                    bounds.y = part.cylinder.diameter;
                    bounds.z = part.cylinder.depth;
                    return bounds;

                }
            }
            bounds.X = 1;
            bounds.Y = 1;
            bounds.Z = 1;
            MessageBox.Show("no bounds\n\nImpossible senario. please report");
            return bounds;
        }
        //Reorganize bounds according to rotation:
        private dynamic flip(dynamic child, string neworder)
        {
            int x = child.bounds.x;
            int y = child.bounds.y;
            int z = child.bounds.z;
            if (neworder == "zxy") //shift
            {
                child.bounds.x = z;
                child.bounds.y = x;
                child.bounds.z = y;
            }
            if (neworder == "yzx") //"
            {
                child.bounds.x = y;
                child.bounds.y = z;
                child.bounds.z = x;
            }
            if (neworder == "xzy") //switch
            {
                child.bounds.x = x;
                child.bounds.y = z;
                child.bounds.z = y;
            }
            if (neworder == "zyx")
            {
                child.bounds.x = z;
                child.bounds.y = y;
                child.bounds.z = x;
            }
            if (neworder == "yxz")
            {
                child.bounds.x = y;
                child.bounds.y = x;
                child.bounds.z = z;
            }
            return child;
        }
        //get new pos and correct ingame bounds for child
        bool missingmod = false;
        private dynamic getposandbounds(dynamic whatever)
        {
            dynamic child = Newtonsoft.Json.JsonConvert.DeserializeObject(Convert.ToString(whatever));
            string uuid = child.shapeId;

            if (child.bounds == null) //add bounds to parts (blocks do not get affected)
            {
                JObject gameblocks = MainWindow.gameblocks;
                if (gameblocks[child.shapeId.Value.ToLower()] != null)
                {
                    child.bounds = getbounds(gameblocks[child.shapeId.Value.ToLower()]);
                }
                else
                {
                    missingmod = true;
                    child.bounds = new JObject();
                    child.bounds.x = 1;
                    child.bounds.y = 1;
                    child.bounds.z = 1;
                }

                //switch bounds here
                if (Math.Abs(Convert.ToInt32(child.xaxis)) == 3)
                {
                    if (Math.Abs(Convert.ToInt32(child.zaxis)) == 2)
                    {
                        flip(child, "yzx");
                    }
                    if (Math.Abs(Convert.ToInt32(child.zaxis)) == 1)
                    {
                        flip(child, "zyx");
                    }
                }
                if (Math.Abs(Convert.ToInt32(child.xaxis)) == 2)
                {
                    if (Math.Abs(Convert.ToInt32(child.zaxis)) == 3)
                    {
                        flip(child, "yxz");
                    }
                    if (Math.Abs(Convert.ToInt32(child.zaxis)) == 1)
                    {
                        flip(child, "zxy");
                    }
                }
                if (Math.Abs(Convert.ToInt32(child.xaxis)) == 1)
                {
                    if (Math.Abs(Convert.ToInt32(child.zaxis)) == 2)
                    {
                        flip(child, "xzy");
                    }
                }
            }
            //this updating pos only applies to parts, blocks do not get affected as they always have xaxis 1 zaxis 3
            if (child.xaxis == -1 | child.zaxis == -1 | (child.xaxis == 2 && child.zaxis == 3) | (child.xaxis == 3 && child.zaxis == -2) | (child.xaxis == -2 && child.zaxis == -3) | (child.xaxis == -3 && child.zaxis == 2))
                child.pos.x -= child.bounds.x;
            if (child.xaxis == -2 | child.zaxis == -2 | (child.xaxis == -1 && child.zaxis == 3) | (child.xaxis == -3 && child.zaxis == -1) | (child.xaxis == 1 && child.zaxis == -3) | (child.xaxis == 3 && child.zaxis == 1))
                child.pos.y -= child.bounds.y;
            if (child.xaxis == -3 | child.zaxis == -3 | (child.xaxis == -2 && child.zaxis == 1) | (child.xaxis == -1 && child.zaxis == -2) | (child.xaxis == 1 && child.zaxis == 2) | (child.xaxis == 2 && child.zaxis == -1))
                child.pos.z -= child.bounds.z;
            return child;
        }
    }
}