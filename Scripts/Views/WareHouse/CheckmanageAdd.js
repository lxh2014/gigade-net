
function addFunction(row, store) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        labelWidth: 120,
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'textfield',
                fieldLabel: "工作編號",
                name: 'jobnumber',
                id: 'jobnumber',
                minValue: 0,
                allowBlank: true
            }
        ],
        buttons: [{
            text: "點擊復盤",
            formBind: true,
            handler: function () {
                var form = this.up('form').getForm();
                Ext.Ajax.request({
                    url: "/WareHouse/FupanComplete",
                    params: {
                        jobnumber: Ext.getCmp('jobnumber').getValue()
                    },
                    success: function (response) {
                        var result = Ext.decode(response.responseText);
                        if (result.success) {
                            if (result.msg == 1)
                            {
                                Ext.Msg.alert(INFORMATION, "復盤成功!");
                                CheckmanageStore.load();
                            }
                            else if (result.msg == -2)
                            {
                                Ext.Msg.alert(INFORMATION, "該工作編號不存在或已被刪除!");
                            }
                            else if (result.msg == -1)
                            {
                                Ext.Msg.alert(INFORMATION, "該工作編號目前不能復盤!");
                            }
                            else if (result.msg == -3)
                            {
                                Ext.Msg.alert(INFORMATION, "所有工作編號都已復盤完成!");
                            }
                        }
                        else {
                            Ext.Msg.alert(INFORMATION, "復盤失敗!");
                        }
                    }
                });
            }
        }]
    });

    var editWin = Ext.create('Ext.window.Window', {
        title: "復盤完成",
        id: 'editWin',
        iconCls: 'icon-user-edit',
        width: 550,
        height: 380,
        autoScroll: true,
        //        height: document.documentElement.clientHeight * 260 / 783,
        layout: 'fit',
        items: [editFrm],
        closeAction: 'destroy',
        modal: true,
        constrain: true,    //窗體束縛在父窗口中
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: CLOSEFORM,
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('editWin').destroy();
                         CheckmanageStore.load();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }]
        ,
        listeners: {
            'show': function () {
                if (row == null) {
                  
                    editFrm.getForm().reset();
                } 
            }
        }
    });
    editWin.show();
}