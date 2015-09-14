editFunction = function (row) {
    var form = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        items: [
            {
                xtype: 'textfield',
                fieldLabel: '活動地址',
                id: 'pageUrl',
                vtype: 'url',
                margin: '20 6 5 7',
                allowBlank: false
            }
        ],
        buttons: [
            {
                text: '保存',
                formBind: true,
                disabled: true,
                handler: function () {
                    var form = this.up('form').getForm();
                    if (form.isValid())
                    {
                        save();
                    }
                }
            }
        ] 

    });

    var editWin = Ext.create('Ext.window.Window', {
        title: '編輯',
        iconCls: 'icon-user-edit',
        width: 350,
       height:150,
       layout: 'fit',
       items:[form],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
               {
                   type: 'close',
                   handler: function (event, toolEl, panel) {
                       Ext.MessageBox.confirm('提示信息', '是否關閉窗口', function (btn) {
                           if (btn == "yes") {
                               editWin.destroy();
                           }
                           else {
                               return false;
                           }
                       });
                   }
               }]
    });
    editWin.show();
    function  save()
    {
      
        var rowIDs = "";
        for (var i = 0; i < row.length; i++) {
            rowIDs += row[i].data.rid + '|';
        }
        //alert(rowIDs);
        //alert(Ext.getCmp('pageUrl').getValue());
        Ext.Ajax.request({
            url: '/PromotionsMaintain/UpdateUrl',
            method: 'post',
            params: {
                rowID: rowIDs,
                pageUrl: Ext.getCmp('pageUrl').getValue()
            },
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                if (result.success) {
                    Ext.Msg.alert("提示信息", "保存成功");
                    editWin.close();
                    ProPromoStore.load();
                }
                else {
                    Ext.Msg.alert("提示信息", "保存失敗");
                    editWin.close();
                    ProPromoStore.load();
                }
            }
        });
    }
}