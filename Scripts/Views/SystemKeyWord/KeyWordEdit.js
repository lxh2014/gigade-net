/*
* 文件名稱 :KeywordEdit.js
* 文件功能描述 :編輯系統關鍵字
* 版權宣告 :鄭州分公司
* 開發人員 : 文博
* 版本資訊 : 1.0
* 日期 : 2015-7-31
* 修改人員 :無
* 版本資訊 : 1.0
* 日期 : 2015-7-31
* 修改備註 :無
*/
var old_key_word;
function EditSaveClick(row, store) {
    var form = this.up('form').getForm();
    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "loading..." });
    var strflag;
    myMask.show();
    if (Ext.getCmp("iskey").getValue()) {
        strflag = '1';
    }
    if (Ext.getCmp("notkey").getValue()) {
        strflag = '0';
    }
    var reg = new RegExp(" ", "g"); //创建正则RegExp对象
    var key_word = Ext.getCmp('key_word').getValue()
    var newkey_word = key_word.replace(reg, '');//將匹配正則的字符替換
    if (newkey_word == "") {
        Ext.Msg.alert(INFORMATION, "關鍵字不能為空!");
        myMask.hide();
        return false;
    }
    if (strflag == '') {
        Ext.Msg.alert(INFORMATION, "請選擇關鍵字是否為食安關鍵字!");
        myMask.hide();
        return false;
    }
    if (form.isValid())
    {
        Ext.Ajax.request({
            url: "/SystemKeyWord/CheckKeyWordExsit",
            params: {
                key_word: newkey_word
            },
            success: function (response) {
                var result = eval("(" + response.responseText + ")");
                if (result.success == true )
                {
                    if (result.msg > 0 && old_key_word != key_word)
                    {
                        myMask.hide();
                        //Ext.getCmp('key_word').setValue("");
                        Ext.Msg.alert(INFORMATION, "此關鍵字已存在!");
                        return false;
                    }
                    else {
                        form.submit({
                            params: {
                                operateType: 2,//操作類型:1:增 2:改
                                flag: strflag,
                                row_id: Ext.getCmp("row_id").getValue(),
                                key_word: newkey_word
                            },
                            success: function (form, action) {
                                myMask.hide();
                                var result = Ext.decode(action.response.responseText);
                                if (result.success) {
                                    if (result.state > 0) {
                                        Ext.Msg.alert("提示信息", "修改成功!");
                                        Ext.getCmp('EditWin').destroy();
                                        var totalcount = SystemKeyStore.getCount();
                                        if (totalcount != 0) {
                                            SystemKeyStore.load();
                                        }
                                    }

                                }
                                else {
                                    Ext.Msg.alert("提示信息", "修改失敗!");
                                }
                            },
                            failure: function (form, action) {
                                myMask.hide();
                                var result = Ext.decode(action.response.responseText);
                                if (result.state == 0) {
                                    Ext.Msg.alert("提示信息", "修改失敗");
                                }
                                else {
                                    Ext.Msg.alert("提示信息", "修改失敗!");
                                }
                            }
                        });
                    }
                }
            }
        });
    }
}


EditFunction = function (row, store) {
    var EditForm = Ext.create('Ext.form.Panel', {
        id: 'EditForm',
        frame: true,
        plain: true,
        layout: 'anchor',
        url: '/SystemKeyWord/SaveSystemKeyWord',
        defaults: { msgTarget: "side" },
        items:
        [
            {
                xtype: 'textfield',
                labelWidth: 60,
                width: 260,
                margin: '10 0',
                id: 'row_id',
                name: 'row_id',
                hidden: true
            },
            {
                xtype: 'textfield',
                fieldLabel: "關鍵字",
                labelWidth: 60,
                width: 260,
                maxLength: 50,
                margin: '10 0',
                id: 'key_word',
                name: 'key_word',
                allowBlank: false,
                listeners: {
                    specialkey: function (field, e) {
                        if (e.getKey() == Ext.EventObject.ENTER) {
                            EditSaveClick();
                        }
                    }
                }
            },
             {
                 xtype: 'fieldcontainer',
                 fieldLabel: '是否食安關鍵字',
                 defaultType: 'radiofield',
                 defaults: {
                     flex: 1
                 },
                 layout: 'hbox',
                 items: [
                     {
                         boxLabel: '是',
                         name: 'flog',
                         id: 'iskey'
                     },
                     {
                         boxLabel: '否',
                         name: 'flog',
                         id: 'notkey'
                     }
                 ]
             },
        ],
        buttons: [
                 {
                     formBind: true,
                     disabled: true,
                     text: "保存",
                     margin: '0 20 10 0',
                     handler: EditSaveClick
                 },
                  {
                      //formBind: true,
                      text: "取消",
                      margin: '0 10 10 0',
                      handler: function () {
                          Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                              if (btn == "yes") {
                                  Ext.getCmp('EditWin').destroy();
                              }
                              else {
                                  return false;
                              }
                          });
                      }
                  }
        ]
    });
    var EditWin = Ext.create('Ext.window.Window', {
        id: 'EditWin',
        width: 300,
        hight: 300,
        resizable: false,
        title: "编辑",
        iconCls: 'icon-user-edit',
        layout: 'fit',
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        closable: false,
        items: [EditForm],
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        tools: [
         {
             type: 'close',
             qtip: "關閉",
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('EditWin').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }
        ],
        listeners: {
            'show': function () {
                EditForm.getForm().loadRecord(row);
                old_key_word = row.data.key_word;
            }
        }
    });
    if (parseInt(row.data.flag) == 0) {
        Ext.getCmp('notkey').setValue(true);
    }
    else {
        Ext.getCmp('iskey').setValue(true);
    }
    EditWin.show();
}