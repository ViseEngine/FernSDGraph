﻿using System;
using System.Collections.Generic;
using System.Linq;
using FernGraph;
using UnityEngine;

namespace FernNPRCore.StableDiffusionGraph
{
    [Serializable]
    public struct PromptData
    {
        public string       word;
        public float        weight;
        public bool         end;
        public float        process;

        public int          color;
        public PromptData SetWeight(float weight)
        {
            this.weight = weight;
            return this;
        }
        public PromptData SetColor(int color)
        {
            this.color = color;
            return this;
        }
        public PromptData SetProcess(float process)
        {
            this.process = process;
            return this;
        }
        public PromptData SetProcessType(bool end)
        {
            this.end = end;
            return this;
        }
    }

    [Serializable]
    public class PromptRegisterData : ScriptableObject
    {
        public List<PromptData> positiveDatas = new List<PromptData>();
        public List<PromptData> negativeDatas = new List<PromptData>();
    }
    
    [Node(Path = "SD Standard")]
    [Tags("SD Node")]
    public class SDPromptRegisterNode : Node
    {
        public PromptRegisterData RegisterData = ScriptableObject.CreateInstance<PromptRegisterData>();
        public Prompt Prompt = new Prompt();
        [Output] public string Positive;
        [Output] public string Negative;

        public override object OnRequestValue(Port port) => port.Name switch
        {
            "Positive" => Prompt.positive,
            "Negative" => Prompt.negative,
            _ => null
        };
    }
}