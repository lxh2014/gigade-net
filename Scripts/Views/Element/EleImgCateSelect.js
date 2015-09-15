
SelectImgCate = function (cid, cname, t_treeCateStore) {



    var treeCatePanel = new Ext.tree.TreePanel({
        id: 'treePanelCate',
        region: 'west',
        border: 0,
        height: 480,
        autoScroll: true,
        store: t_treeCateStore,
        listeners: {
            'itemclick': function (view, record, item, index, e) {
                nodeId = record.raw.id; //获取点击的节点id
                nodeText = record.raw.text; //获取点击的节点text
                Ext.getCmp('c_id').setValue(nodeId + '--' + nodeText);
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
                  fieldLabel: CAETIMG,
                  id: 'c_id',
                  name: 'c_id',
                  width: 300,
                  margin: '0 10 0 0',
                  hidden: false
              },
                {
                    xtype: 'button',
                    text: CLEAR,
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
                Ext.getCmp('category_id').setValue(cate.split('--')[0]);
                Ext.getCmp('category_name').setValue(cate.split('--')[1]);
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
        title: AREATITLE,
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
                    Ext.getCmp('c_id').setValue(cid + '--' + cname);
                    var record = treeCatePanel.getStore().getNodeById(cid);
                    treeCatePanel.getSelectionModel().select(record);
                }
            }
        }
    });
    editWinCate.show();


}
