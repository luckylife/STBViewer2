﻿using OpenTK.Mathematics;
using STBViewer2_202.MainWindow;
using STBViewer2_202.ST_BRIDGE202;
using STBViewer2Lib;
using STBViewer2Lib.DetailsWindow;
using STBViewer2Lib.MainWindow;
using STBViewer2Lib.OpenGL;
using STBViewer2Lib.SettingWindow;
using STBViewer2Lib.Shaders;
using System.Xml.Serialization;
namespace ST_BRIDGE202
{
    public partial class StbPost : IModelElement_202
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
            return CategorySetting.FromMediaColor(settings.StbPostColor);
        }

        public void SetElementEnable(CategorySetting settings)
        {
            isEnable = settings.ShowStbPost;
        }

        public void InitilizeModel(IST_BRIDGE istBridge, ShaderLoader shader)
        {
            ST_BRIDGE? stBridge = istBridge as ST_BRIDGE;
            StbNode bottom = stBridge.StbModel.StbNodes.First(n => n.id == this.id_node_bottom);
            StbNode top = stBridge.StbModel.StbNodes.First(n => n.id == this.id_node_top);
            OutlineModel.Add(new Wire(new Vector3((float)(bottom.X + this.offset_bottom_X) * AbstractModelManager.ScaleFactor, (float)(bottom.Y + offset_bottom_Y) * AbstractModelManager.ScaleFactor, (float)(bottom.Z + offset_bottom_Z) * AbstractModelManager.ScaleFactor),
                new Vector3((float)(top.X + offset_top_X) * AbstractModelManager.ScaleFactor, (float)(top.Y + offset_top_Y) * AbstractModelManager.ScaleFactor, (float)(top.Z + offset_top_Z) * AbstractModelManager.ScaleFactor), shader)); // スケール変換後の座標で初期化
            AnalysisNodes.Add(new Sphere((float)bottom.X * AbstractModelManager.ScaleFactor, (float)bottom.Y * AbstractModelManager.ScaleFactor, (float)bottom.Z * AbstractModelManager.ScaleFactor, 0.1f, shader));
            AnalysisNodes.Add(new Sphere((float)top.X * AbstractModelManager.ScaleFactor, (float)top.Y * AbstractModelManager.ScaleFactor, (float)top.Z * AbstractModelManager.ScaleFactor, 0.1f, shader));
        }

        public List<IPropertyTab> GetAdditionalDetails(IST_BRIDGE istBridge)
        {
            ST_BRIDGE? stBridge = istBridge as ST_BRIDGE;
            List<IPropertyTab> tabs = [];
            List<PropertyDetail> properties = [];
            if (kind_structure.ToString() == "RC")
            {
                StbSecColumn_RC rc = stBridge.StbModel.StbSections.StbSecColumn_RC.First(s => s.id == id_section);
                properties = ((IModelElement)this).GetPropertyDetail(rc, istBridge);
            }
            else if (kind_structure.ToString() == "S")
            {
                StbSecColumn_S s = stBridge.StbModel.StbSections.StbSecColumn_S.First(s => s.id == id_section);
                properties = ((IModelElement)this).GetPropertyDetail(s, istBridge);
            }
            else if (kind_structure.ToString() == "SRC")
            {
                StbSecColumn_SRC src = stBridge.StbModel.StbSections.StbSecColumn_SRC.First(s => s.id == id_section);
                properties = ((IModelElement)this).GetPropertyDetail(src, istBridge);
            }
            else if (kind_structure.ToString() == "CFT")
            {
                StbSecColumn_CFT cft = stBridge.StbModel.StbSections.StbSecColumn_CFT.First(s => s.id == id_section);
                properties = ((IModelElement)this).GetPropertyDetail(cft, istBridge);

            }
            tabs.Add(new PropertySection("断面", properties));

            if (joint_id_bottom != null || joint_id_top != null)
            {
                List<PropertyDetail> jointProperties = [];
                if (joint_id_bottom != null)
                {
                    StbJointColumnShapeH? shapeH = stBridge.StbModel.StbJoints?.StbJointColumnShapeH.FirstOrDefault(j => j.id == joint_id_bottom);
                    if (shapeH != null)
                    {
                        jointProperties.AddRange(((IModelElement)this).GetPropertyDetail(shapeH, istBridge));
                    }

                    StbJointColumnShapeT? shapeT = stBridge.StbModel.StbJoints?.StbJointColumnShapeT.FirstOrDefault(j => j.id == joint_id_bottom);
                    if (shapeT != null)
                    {
                        jointProperties.AddRange(((IModelElement)this).GetPropertyDetail(shapeT, istBridge));
                    }

                    StbJointColumnShapeCross? crossH = stBridge.StbModel.StbJoints?.StbJointColumnShapeCross.FirstOrDefault(j => j.id == joint_id_bottom);
                    if (crossH != null)
                    {
                        jointProperties.AddRange(((IModelElement)this).GetPropertyDetail(crossH, istBridge));
                    }
                }
                if (joint_id_top != null)
                {
                    StbJointColumnShapeH? shapeH = stBridge.StbModel.StbJoints?.StbJointColumnShapeH.FirstOrDefault(j => j.id == joint_id_top);
                    if (shapeH != null)
                    {
                        jointProperties.AddRange(((IModelElement)this).GetPropertyDetail(shapeH, istBridge));
                    }

                    StbJointColumnShapeT? shapeT = stBridge.StbModel.StbJoints?.StbJointColumnShapeT.FirstOrDefault(j => j.id == joint_id_top);
                    if (shapeT != null)
                    {
                        jointProperties.AddRange(((IModelElement)this).GetPropertyDetail(shapeT, istBridge));
                    }

                    StbJointColumnShapeCross? crossH = stBridge.StbModel.StbJoints?.StbJointColumnShapeCross.FirstOrDefault(j => j.id == joint_id_top);
                    if (crossH != null)
                    {
                        jointProperties.AddRange(((IModelElement)this).GetPropertyDetail(crossH, istBridge));
                    }
                }
                tabs.Add(new PropertySection("継手", jointProperties));
            }

            return tabs;
        }
    }
}
