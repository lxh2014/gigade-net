Ext.onReady(function () {
    createForm();
});

function createForm() {
    //tab
    var center = Ext.create('Ext.tab.Panel', {
        id: 'ContentPanel',
        region: 'center',
        margins: '0 5 0 0',
        activeTab: 0,
        items: [{
            title: TOP1,
            html: rtnFrame('Channel/Channel')
        }, {
            title: TOP2,
            html: rtnFrame('Channel/ChannelContact')
        }, {
            title: TOP3,
            html: rtnFrame('Channel/ChannelShipping')
        }, {
            title: TOP4,
            html: rtnFrame('Channel/ChannelOther')
        }]
    });

    Ext.create('Ext.Viewport', {
        layout: 'border',
        items: [
            center
        ]
    })
}

function setChannelId(value) {
    $("#txt_channelId").val(value);
}
function getChannelId() {
    return $("#txt_channelId").val();
}

function rtnFrame(url) {
    return "<iframe id='if_channel' frameborder=0 width=100% height=100% src='" + url + "' ></iframe>";
}