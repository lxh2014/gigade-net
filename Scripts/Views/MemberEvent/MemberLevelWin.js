
showMemberLevelForm = function (row)
{   
    code = "";
    var form = Ext.widget('form', {
        border: false,
        frame: true,
        bodyPadding: 10,
        layout: 'anchor',
        fieldDefaults: {
            labelAlign: 'left',
            labelWidth: 120,
            anchor: '95%'
        },
        items: [
                 {
                     xtype: 'fieldcontainer',
                     layout: 'hbox',
                     width: 500,
                     items: [
                        {
                            xtype: 'checkboxgroup',
                            fieldLabel: '級別選定',
                            columns: 2,
                            id: 'lel',
                            name: 'lel',
                            width: 300,
                            vertical: false,
                            allowBlank: false,
                            items:levelitems
                            //items: [
                            //    { boxLabel: '一般會員', name: 'level', inputValue: 'N' },
                            //    { boxLabel: '銅牌會員', name: 'level', inputValue: 'C' },
                            //    { boxLabel: '銀牌會員', name: 'level', inputValue: 'S' },
                            //    { boxLabel: '金牌會員', name: 'level', inputValue: 'G' },
                            //    { boxLabel: '鑽石會員', name: 'level', inputValue: 'D' }
                            //]
                        }
                     ]
                 }
        ],
        buttons: [{
            text: SAVE,
            formBind: true,
            disabled: true,
            handler: function ()
            {
                var form = this.up('form').getForm();
                if (form.isValid())
                {
                    var level = Ext.getCmp('lel').getChecked();
                    //群組級別
                    Ext.Array.each(level, function (item)
                    {
                        code += item.inputValue + ',';
                    });
                    MemberLevelWin.close();
                }
            }
        }]
    });


    MemberLevelWin = Ext.widget('window', {
        title: '會員級別選定',
        closeAction: 'destroy',
        width: 400,
        layout: 'anchor',
        constrain: true,
        resizable: false,
        modal: true,
        items: form,
        listeners: {
            'show': function ()
            {
                if (row != null)
                {
                    if (row.data.ml_code.toString() != "0")
                    {
                        var ml_code = row.data.ml_code.toString().split(",");
                        Ext.getCmp('lel').setValue({
                            level: ml_code
                        });
                    }
                }
            }
        }
    });
    MemberLevelWin.show();
    //loadData(condition_id);
}

