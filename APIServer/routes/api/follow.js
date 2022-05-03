const express = require('express');
const Music = require('../../models/music');
const User = require('../../models/user');
const auth = require('../../middleware/auth');

const router = express.Router();

router.get('/follow', auth, async(req, res)=>{
    const id = req.user.id;

    User.findOne({id:id}).then((user)=>{
        res.status(200).json({follow:user.follow})
    })
})

router.get('/follower', auth, async(req, res)=>{
    const id = req.user.id;

    User.findOne({id:id}).then((user)=>{
        res.status(200).json({follower:user.follower})
    })
})

router.post('/', auth, async(req, res)=>{
    const {userId} = req.body;
    const id = req.user.id;

    await User.updateOne({id: id}, {$addToSet: { follow: userId}});
    await User.updateOne({id: userId}, {$addToSet: { follower: id}});
    res.status(200).json({message:"OK"})

})

router.post('/delete', auth, async(req, res)=>{
    const {userId} = req.body;
    const id = req.user.id;

    await User.updateOne({id: id}, {$pull: { follow: userId}});
    await User.updateOne({id: userId}, {$pull: { follower: id}});
    res.status(200).json({message:"OK"})

})

module.exports = router;