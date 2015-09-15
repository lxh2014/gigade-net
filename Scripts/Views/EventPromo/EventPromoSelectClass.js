
SelectImgCate = function (cid, cname, t_treeCateStore) {
    var endcategorysname = "";
    var endcategorysid = "";
    var treeCatePanel = new Ext.tree.TreePanel({
        id: 'treePanelCate',
        region: 'west',
        border: 0,
        height: 480,
        multiSelect: true,
        autoScroll: true,
        store: t_treeCateStore,
        listeners: {
            'selectionchange': function (value, selectd, e) {
                var results = "";
                var categorysid = "";
                var categorysname = "";
               var lenght =Ext.getCmp("treePanelCate").getSelectionModel().getSelection().length;
               for (var i = 0; i < lenght; i++)
               {
                   nodeId = selectd[i].raw.id; //获取点击的节点id
                   nodeText = selectd[i].raw.text; //获取点击的节点text
                   results = results + nodeId + '--' + nodeText + ',';
                   categorysid = categorysid + nodeId + ',';
                   categorysname =categorysname+ nodeText + ',';
               }
               var endresult = results.substring(0, results.lastIndexOf(','));
               endcategorysid = categorysid.substring(0, categorysid.lastIndexOf(','));
               endcategorysname = categorysname.substring(0, categorysname.lastIndexOf(','));
               //nodeId = record.raw.id; //获取点击的节点id
               //nodeText = record.raw.text; //获取点击的节点text
                //Ext.getCmp('c_id').setValue(nodeId + '--' + nodeText);
               Ext.getCmp('c_id').setValue(endresult);
            }
        }
    });

    var Frm = Ext.create('Ext.form.Panel', {
        id: 'Frm',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        labelWidth: 20,
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
              {
                  xtype: 'fieldcontainer',
                  layout: 'hbox',
                  items: [
              {
                  xtype: 'textfield',
                  fieldLabel: "類別",
                  id: 'c_id',
                  name: 'c_id',
                  width: 300,
                  margin: '0 10 0 0',
                  hidden: false
              },
                {
                    xtype: 'button',
                    text: "清空",
                    handler: function () {
                        Ext.getCmp('c_id').setValue("");
                    }
                }
                  ]
              },
       treeCatePanel
        ],
        buttons: [{
            text: "確定",
            id: 'closeBtn',
            formBind: true,
            disabled: true,
            handler: function () {
                var cate = Ext.getCmp('c_id').getValue();
                Ext.getCmp('category_id').setValue(endcategorysid);
                Ext.getCmp('category_name').setValue(endcategorysname);
                if (cate != "") {
                    Ext.getCmp('category_name').show();
                } else {
                    Ext.getCmp('category_name').hide();
                }
                editWinCate.destroy();
            }
        }]
    });
    var editWinCate = Ext.create('Ext.window.Window', {
        title: "類別設定",
        id: 'editWinCate',
        iconCls: 'icon-user-edit',
        width: 420,
        height: 600,
        layout: 'fit',
        items: [Frm],
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        constrain: true,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        listeners: {
            'show': function () {
                if (cid != 0 && cid != null && cname != null) {
                    var cidstr = cid.toString().split(",");
                    var cnamestr = cname.toString().split(",");
                    var cidcname="";
                    for(var i=0;i<cidstr.length;i++)
                    {
                        cidcname=cidcname+cidstr[i]+'--'+cnamestr[i]+',';
                    }
                    var endcidname = cidcname.substring(0, cidcname.lastIndexOf(','));
                    Ext.getCmp('c_id').setValue(endcidname);
                    var recordlist = new Array();
                    for (var j = 0; j < cidstr.length; j++)
                    {
                        var record = treeCatePanel.getStore().getNodeById(cidstr[j]);
                        recordlist.push(record);
                    }
                    treeCatePanel.getSelectionModel().select(recordlist);
                }
            }
        }
    });
    editWinCate.show();


}
