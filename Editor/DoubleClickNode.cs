using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

namespace TheKiwiCoder {
    public class  DoubleClickNode : MouseManipulator {

        double time;
        double doubleClickDuration = 0.3;

        public DoubleClickNode() {
            time = EditorApplication.timeSinceStartup;
        }

        protected override void RegisterCallbacksOnTarget() {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        protected override void UnregisterCallbacksFromTarget() {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        }

        private void OnMouseDown(MouseDownEvent evt) {
            if (!CanStopManipulation(evt))
                return; 

            NodeView clickedElement = evt.target as NodeView;
            if (clickedElement == null) {
                var ve = evt.target as VisualElement;
                clickedElement = ve.GetFirstAncestorOfType<NodeView>();
                if (clickedElement == null)
                    return;
            }

            double duration = EditorApplication.timeSinceStartup - time;
            if (duration < doubleClickDuration) {
                OnDoubleClick(evt, clickedElement);
                evt.StopImmediatePropagation();
            }

            time = EditorApplication.timeSinceStartup;
        }

        void OpenScriptForNode(NodeView clickedElement) {

            var script = EditorUtility.GetNodeScriptPath(clickedElement);
            if (script) {
                // Open script in the editor:
                AssetDatabase.OpenAsset(script);
                
                // Remove the node from selection to prevent dragging it around when returning to the editor.
                BehaviourTreeEditorWindow.Instance.treeView.RemoveFromSelection(clickedElement);
            }
        }

        void OpenSubtree(NodeView clickedElement) {
            BehaviourTreeEditorWindow.Instance.PushSubTreeView(clickedElement.node as SubTree);
        }

        void OnDoubleClick(MouseDownEvent evt, NodeView clickedElement) {
            if (clickedElement.node is SubTree) {
                OpenSubtree(clickedElement);
            } else {
                OpenScriptForNode(clickedElement);
            }
        }
    }
}