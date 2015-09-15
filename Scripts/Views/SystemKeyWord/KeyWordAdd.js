/*
* 文件名稱 :KeywordAdd.js
* 文件功能描述 :新增系統關鍵字
* 版權宣告 :鄭州分公司
* 開發人員 : 文博
* 版本資訊 : 1.0
* 日期 : 2015-7-31
* 修改人員 :無
* 版本資訊 : 1.0
* 日期 : 2015-7-31
* 修改備註 :無
*/
function AddSaveClick() {
    var form = this.up('form').getForm();
    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "loading..." });
    var strflag;
    myMask.show();
    var form = this.up('form').getForm();
    if (Ext.getCmp("iskey").getValue()) {
        strflag = '1';
    }
    if (Ext.getCmp("notkey").getValue()) {
        strflag = '0';
    }
    var reg = new RegExp(" ", "g"); //创建正则RegExp对象
    var key_word = Ext.getCmp('key_word').getValue()
    var newkey_word = key_word.replace(reg, '');//將匹配正則的字符替換
    if (newkey_word =="")
    {
        Ext.Msg.alert(INFORMATION, "關鍵字不能為空!");
        myMask.hide();
        return false;
    }
    if (strflag =='')
    {
        Ext.Msg.alert(INFORMATION, "請選擇關鍵字是否為食安關鍵字!");
    }
    if (form.isValid()) {
        Ext.Ajax.request({
            url: "/SystemKeyWord/CheckKeyWordExsit",
            params: {
                key_word: newkey_word
            },
            success: function (response) {
                var result = eval("(" + response.responseText + ")");
                if (result.success == true) {
                    if (result.msg > 0) {
                        myMask.hide();
                        Ext.getCmp('key_word').setValue("");
                        Ext.Msg.alert(INFORMATION, "此關鍵字已存在!");
                        return false;
                    }
                    else {
                        form.submit({
                            params: {
                                operateType: 1,//操作類型:1:增 2:改
                                flag: strflag,
                                key_word: Ext.getCmp("key_word").getValue()
                            },
                            success: function (form, action) {
                                myMask.hide();
                                var result = Ext.decode(action.response.responseText);
                                if (result.success) {
                                    if (result.state > 0) {
                                        Ext.getCmp('AddWin').destroy();
                                        Ext.Msg.alert("提示信息", "添加成功!");
                                        var totalcount = SystemKeyStore.getCount();
                                        if (totalcount != 0) {
                                            SystemKeyStore.load();
                                        }
                                    }
                                    else {
                                        Ext.Msg.alert("提示信息", "添加失敗!");
                                    }
                                }
                            },
                            failure: function (form, action) {
                                myMask.hide();
                                var result = Ext.decode(action.response.responseText);
                                if (result.state == 0) {
                                    Ext.Msg.alert("提示信息", "添加失敗");
                                }
                                else {
                                    Ext.Msg.alert("提示信息", "添加失敗!");
                                }
                            }
                        });
                    }
                }
            }
        });
    }
}

AddFunction = function () {
    var AddForm = Ext.create('Ext.form.Panel', {
        id: 'AddForm',
        frame: true,
        plain: true,
        layout: 'anchor',
        url: '/SystemKeyWord/SaveSystemKeyWord',
        defaults: { msgTarget: "side" },
        items:
        [
            {
                xtype: 'textfield',
                fieldLabel: "關鍵字",
                labelWidth: 60,
                width: 260,
                margin: '10 0',
                id: 'key_word',
                name: 'key_word',
                maxLength: 50,
                maxLengthText: '關鍵字最多為25個中文字或者50個英文字',
                allowBlank: false,
                enableKeyEvents: true,
                scope: this,
                listeners: {
                    specialkey: function (field, e) {
                        if (e.getKey() == Ext.EventObject.ENTER) {
                                 AddSaveClick();
                        }
                    },
                    keyup: { //超過五十自動取前50字符并提示；add by xiaohui 2015/08/07
                        fn: function (o, evt)
                        {
                            var content = Ext.getCmp('key_word').getValue();
                            var sum = 0;
                            for (var i = 0; i < content.length; i++)
                            {
                                if ((content.charCodeAt(i) >= 0) && (content.charCodeAt(i) <= 255))
                                {
                                    sum = sum + 1;
                                } else
                                {
                                    sum = sum + 2; // 漢字占2个字符  
                                }
                                if (sum > 50)  
                                {
                                    content = content.substring(0,i);  //start 0 end i
                                    Ext.getCmp('key_word').setValue(content);
                                    Ext.Msg.alert(INFORMATION, '關鍵字最多為25個中文字或50個英文字');
                                }
                            }
                            
                        },
                        scope: this
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
                         id: 'iskey',
                         checked: false
                     },
                     {
                         boxLabel: '否',
                         name: 'flog',
                         id: 'notkey',
                         checked: true
                     }
                 ]
             },
        ],
        buttons: [
                {
                    formBind: true,
                    disabled: true,
                    text: "保存",
                    margin: '0 20 0 0',
                    handler: AddSaveClick
                },
                  {
                      //formBind: true,
                      text: "取消",
                      margin: '0 10 10 0',
                      handler: function () {
                          Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                              if (btn == "yes") {
                                  Ext.getCmp('AddWin').destroy();
                              }
                              else {
                                  return false;
                              }
                          });
                      }
                  }
        ]
    });
    var AddWin = Ext.create('Ext.window.Window', {
        id: 'AddWin',
        width: 300,
        hight: 300,
        title: "新增",
        resizable: false,
        iconCls: 'icon-user-add',
        layout: 'fit',
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        closable: false,
        items: [AddForm],
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        tools: [
         {
             type: 'close',
             qtip: "關閉",
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('AddWin').destroy();
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
            }
        }
    });
    AddWin.show();
}