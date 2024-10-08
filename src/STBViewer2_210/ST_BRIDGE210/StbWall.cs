﻿using OpenTK.Mathematics;
using STBViewer2_210.ST_BRIDGE210;
using STBViewer2Lib;
using STBViewer2Lib.DetailsWindow;
using STBViewer2Lib.MainWindow;
using STBViewer2Lib.OpenGL;
using STBViewer2Lib.SettingWindow;
using STBViewer2Lib.Shaders;
using System.Xml.Serialization;
namespace ST_BRIDGE210
{
    public partial class StbWall : IModelElement_210
    {
        [XmlIgnore]
        public List<IRender> OutlineModel { get; set; } = [];

        [XmlIgnore]
        public bool isSelected { get; set; }

        [XmlIgnore]
        public bool isEnable { get; set; }

        [XmlIgnore]
        public List<Sphere> AnalysisNodes { get; set; } = [];

        [XmlIgnore]
        public List<IPropertyTab> Tabs { get; set; } = [];

        public Color4 GetElementColor(CategorySetting settings)
        {
            return CategorySetting.FromMediaColor(settings.StbWallColor);
        }

        public void SetElementEnable(CategorySetting settings)
        {
            isEnable = settings.ShowStbWall;
        }

        public void InitilizeModel(IST_BRIDGE istBridge, ShaderLoader shader)
        {
            ST_BRIDGE? stBridge = istBridge as ST_BRIDGE;
            List<Vector3> vertices = [];
            List<List<Vector3>> holes = [];
            string[] ids = this.StbNodeIdOrder.Split(' ');
            for (int i = 0; i < ids.Length; i++)
            {
                StbNode node = stBridge.StbModel.StbNodes.First(n => n.id == ids[i]);
                if (StbWallOffsetList == null)
                {
                    vertices.Add(new Vector3((float)node.X * AbstractModelManager.ScaleFactor, (float)node.Y * AbstractModelManager.ScaleFactor, (float)node.Z * AbstractModelManager.ScaleFactor));
                }
                else
                {
                    vertices.Add(new Vector3((float)(node.X + this.StbWallOffsetList[i].offset_X) * AbstractModelManager.ScaleFactor, (float)(node.Y + this.StbWallOffsetList[i].offset_Y) * AbstractModelManager.ScaleFactor, (float)(node.Z + this.StbWallOffsetList[i].offset_Z) * AbstractModelManager.ScaleFactor));
                }
                AnalysisNodes.Add(new Sphere((float)node.X * AbstractModelManager.ScaleFactor, (float)node.Y * AbstractModelManager.ScaleFactor, (float)node.Z * AbstractModelManager.ScaleFactor, 0.1f, shader));
            }

            /*
            if (StbOpenIdList != null)
            {
                if (vertices == null || vertices.Count < 3)
                {
                    throw new ArgumentException("verticesには少なくとも3つの点が必要です。");
                }

                // 1点目と2点目、最終点の座標
                Vector3 v1 = vertices[0]; // 1点目
                Vector3 v2 = vertices[1]; // 2点目
                Vector3 vn = vertices[^1]; // 最終点

                // XVectorを定義（1点目から2点目へのベクトル）
                Vector3 XVector = (v2 - v1).Normalized();

                // 1点目、2点目、最終点が定義する平面の法線ベクトルを計算
                Vector3 normalVector = Vector3.Cross(v2 - v1, vn - v1).Normalized();

                // XVectorと法線ベクトルから、YVectorを計算
                Vector3 YVector = Vector3.Cross(normalVector, XVector).Normalized();

                // 1点目から最終点に向かうベクトルとYVectorの内積が正の方向か確認
                if (Vector3.Dot(YVector, vn - v1) < 0)
                {
                    // YVectorの方向が負の場合、反転させる
                    YVector = -YVector;
                }
                foreach (StbOpenId? id in StbOpenIdList)
                {
                    StbOpen open = stBridge.StbModel.StbMembers.StbOpens.First(o => o.id == id.id);
                    Vector3 first = vertices[0] + (XVector * (float)open.position_X * AbstractModelManager.ScaleFactor) + (YVector * (float)open.position_Y * AbstractModelManager.ScaleFactor);
                    Vector3 second = first + (XVector * (float)open.length_X * AbstractModelManager.ScaleFactor);
                    Vector3 third = second + (YVector * (float)open.length_Y * AbstractModelManager.ScaleFactor);
                    Vector3 fourth = first + (YVector * (float)open.length_Y * AbstractModelManager.ScaleFactor);
                    holes.Add([first, second, third, fourth]);
                }
            }
            */
            OutlineModel.Add(new Plane(vertices, holes, shader)); // スケール変換後の座標で初期化
        }

        public List<IPropertyTab> GetAdditionalDetails(IST_BRIDGE istBridge)
        {
            ST_BRIDGE? stBridge = istBridge as ST_BRIDGE;
            List<IPropertyTab> tabs = [];
            List<PropertyDetail> properties = [];
            if (kind_structure.ToString() == "RC")
            {
                StbSecWall_RC rc = stBridge.StbModel.StbSections.StbSecWall_RC.First(s => s.id == id_section);
                properties = ((IModelElement)this).GetPropertyDetail(rc, istBridge);
            }
            tabs.Add(new PropertySection("断面", properties));

            /*
            if (StbOpenIdList != null)
            {
                List<PropertyDetail> holes = [];
                int index = 0;
                foreach (StbOpenId? id in StbOpenIdList)
                {
                    StbOpen open = stBridge.StbModel.StbMembers.StbOpens.First(o => o.id == id.id);
                    PropertyDetail property = new("StbOpen", index.ToString())
                    {
                        Children = IModelElement_210.GetPropertyDetail(open)
                    };
                    for (int i = 0; i < property.Children.Count(); i++)
                    {
                        if (property.Children.ElementAt(i).PropertyName == "id_section")
                        {
                            StbSecOpen_RC secOpen = stBridge.StbModel.StbSections.StbSecOpen_RC.First(s => s.id == open.id_section);
                            property.Children.ElementAt(i).Children = IModelElement_210.GetPropertyDetail(secOpen);
                        }
                    }

                    holes.Add(property);
                    index++;
                }
                tabs.Add(new PropertySection("開口", holes));
            }
            */

            return tabs;
        }

    }
}
