
editFunction = function (row, store) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        labelWidth: 45,
        url: '/Vendor/UpdateImage',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
       {
           xtype: 'displayfield',
           id: 'image_filename',
           hidden: true
       },
       {
           xtype: 'label',
           margin:'0 0 0 105',
           html: '<span style="color:red">※排序值越大，產品圖顯示位置越前面。<span>'
       },

  {
      xtype: "numberfield",
      fieldLabel: '排序',
      id: 'image_sort',
      allowBlank: false,
      submitValue: true,

      minValue: 0,
      listeners: {
          afterRender: function (combo) {
              combo.setValue(0);
          }
      }
  },
         {

             xtype: 'radiogroup',
             id: 'image_state',
             fieldLabel: "顯示/隱藏",
             colName: 'Sex_type',

             width: 150,
             defaults: {
                 name: 'Tax_Type'
             },
             columns: 2,
             vertical: true,
             items: [
                { id: 'id1', boxLabel: "顯示", inputValue: '1', checked: true },
                { id: 'id2', boxLabel: "隱藏", inputValue: '2'}]

         }


    ],
        buttons: [{
            text: SAVE,
            formBind: true,
            disabled: true,
            handler: function () {

                var form = this.up('form').getForm();
                if (form.isValid()) {

                    form.submit({

                        params: {
                            image_filename: Ext.htmlEncode(Ext.getCmp('image_filename').getValue()),
                            image_sort: Ext.htmlEncode(Ext.getCmp('image_sort').getValue()),
                            image_state: Ext.htmlEncode(Ext.getCmp('image_state').getValue().Tax_Type)

                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);

                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, SUCCESS);
                                ImageStore.load();
                                editWin.close();
                            } else {
                                Ext.Msg.alert(ERRORSHOW + result.success);
                                editWin.close();
                            }
                        },
                        failure: function () {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                    });
                }

            }
        }]
    });

    var editWin = Ext.create('Ext.window.Window', {
        title: '圖檔維護',
        iconCls: 'icon-user-edit',
        id: 'editWin',
        width: 450,
        //        height: document.documentElement.clientHeight * 260 / 783,
        layout: 'fit',
        items: [editFrm],
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: '是否關閉',
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('editWin').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }],

        listeners: {
            'show': function () {
                if (row == null) {
                    editFrm.getForm().reset(); //如果是添加的話
                }
                else {//編輯
                    editFrm.getForm().loadRecord(row);



                    var imagestate = row.data.image_state.toString();
                    if (imagestate == "1") {
                        Ext.getCmp('id1').setValue(true);
                        Ext.getCmp('id2').setValue(false);
                    } else {
                        Ext.getCmp('id1').setValue(false);
                        Ext.getCmp('id2').setValue(true);
                    }
                }
            }
        }
    });
    editWin.show();



}