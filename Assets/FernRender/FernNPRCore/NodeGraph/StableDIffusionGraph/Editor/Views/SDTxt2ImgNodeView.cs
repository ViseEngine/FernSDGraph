using System;
using System.Collections;
using System.Collections.Generic;
using FernNPRCore.SDNodeGraph;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using GraphProcessor;

[NodeCustomEditor(typeof(SDTxt2ImgNode))]
public class SDTxt2ImgNodeView : SDNodeView
{
	private SDTxt2ImgNode node;
	private DropdownField samplerMethodDropdown;
	private LongField longLastField;
	private ProgressBar progressBar;
	private TextField savePathTxtField;

	public override void Enable()
	{
		base.Enable();
		node = nodeTarget as SDTxt2ImgNode;
		
		if (node == null) return;
		
		List<string> samplerMethodList = new List<string>();
		samplerMethodList.AddRange(SDGraphResource.SdGraphDataHandle.samplers);

		samplerMethodDropdown = new DropdownField(samplerMethodList, 0);
		samplerMethodDropdown.RegisterValueChangedCallback(e =>
		{
			node.samplerMethod = e.newValue;
		});
		samplerMethodDropdown.style.flexGrow = 1;
		samplerMethodDropdown.style.maxWidth = 140;
		
		var label = new Label("Method");
		label.style.width = StyleKeyword.Auto;
		label.style.marginRight = 5;
		
		var containerSampleMethod = new VisualElement();
		containerSampleMethod.style.flexDirection = FlexDirection.Row;
		containerSampleMethod.style.alignItems = Align.Center;
		containerSampleMethod.Add(label);
		containerSampleMethod.Add(samplerMethodDropdown);
		
		extensionContainer.Add(containerSampleMethod);
		
		// last seed
		var labelLastSeed = new Label("Last Seed");
		labelLastSeed.style.width = StyleKeyword.Auto;
		labelLastSeed.style.marginRight = 5;
            
		longLastField = new LongField();
		longLastField.value = node.outSeed;
		longLastField.style.flexGrow = 1;
		longLastField.style.maxWidth = 160;
		var containerLastSeed = new VisualElement();
		containerLastSeed.style.flexDirection = FlexDirection.Row;
		containerLastSeed.style.alignItems = Align.Center;
		containerLastSeed.Add(labelLastSeed);
		containerLastSeed.Add(longLastField);
		extensionContainer.Add(containerLastSeed);
		
		// Save Path
		var savePath = new Label("Auto Save");
		savePath.style.width = StyleKeyword.Auto;
		savePath.style.marginRight = 5;
		savePathTxtField = new TextField();
		savePathTxtField.value = node.savePath;
		savePathTxtField.style.flexGrow = 1;
		savePathTxtField.style.maxWidth = 160;
		var savePathBtn = new Button(SavePathBtn);
		savePathBtn.style.backgroundImage = SDTextureHandle.OpenFolderIcon;
		savePathBtn.style.width = 20;
		savePathBtn.style.height = 20;				
		savePathBtn.style.right = 3;

		var containersavePath = new VisualElement();
		containersavePath.style.flexDirection = FlexDirection.Row;
		containersavePath.style.alignItems = Align.Center;
		containersavePath.Add(savePath);
		containersavePath.Add(savePathTxtField);
		containersavePath.Add(savePathBtn);
		extensionContainer.Add(containersavePath);

		progressBar = new ProgressBar();
		progressBar.highValue = 1;
		node.onProgressStart = null;
		node.onProgressUpdate = null;
		node.onProgressFinish = null;
		node.onProgressStart += OnProgressBarStart;
		node.onProgressUpdate += OnUpdateProgressBar;
		node.onProgressFinish += OnProgressBarFinish;
		
		RefreshExpandedState();
	}

	private void SavePathBtn()
	{
		string path;
		if (string.IsNullOrEmpty(node.savePath))
		{
			path = EditorUtility.SaveFilePanel("Save texture as PNG", "Assets", $"img_preview.png", "png");
		}
		else
		{
			path = EditorUtility.SaveFilePanel("Save texture as PNG", node.savePath, $"img_preview.png", "png");
		}
		savePathTxtField.value = path;
		node.savePath = path;
	}

	private void OnUpdateProgressBar(float progress)
	{
		var total = node.pre_step_count / node.speed;
		var re = (long)((1 - progress) * total * node.job_no_count);
		var str_time = $"{re.Seconds_To_HMS()}";
		var str_speed = node.speed > 1 ? $"{node.speed:F1}it/s" : $"{(1 / node.speed):F3}s/it";
		var str_progress = $"{node.progress * 100:F1}%";
		progressBar.title = $"{str_progress} << {str_time} << {str_speed}";
		progressBar.value = progress;
	}
	
	private void OnProgressBarStart()
	{
		progressBar.value = 0;
		node.progress = 0;
		progressBar.title = $"{node.progress * 100:F1}%";
		progressBar.focusable = true;
		if (!extensionContainer.Contains(progressBar))
		{
			extensionContainer.Add(progressBar);
		}
		RefreshExpandedState();
	}
	
	private void OnProgressBarFinish()
	{
		progressBar.value = 1;
		progressBar.focusable = false;
		if (extensionContainer.Contains(progressBar))
		{
			extensionContainer.Remove(progressBar);
		}
		RefreshExpandedState();
	}
}