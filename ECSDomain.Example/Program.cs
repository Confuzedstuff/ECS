﻿using ECSDomain;

var ecs = new ECS();

ecs.Register(typeof(Program).Assembly);
ecs.InitSystems();

var isRunning = true;
while (isRunning)
{
    var delta = 1 / 60f;
    ecs.UpdateSystems(delta);
}