# sample bezier groups code:

helper tool [http://cubic-bezier.com/?a=1#.79,0,.79,1.5](http://cubic-bezier.com/?a=1#.79,0,.79,1.5)

```csharp
return beziers(b => b.from(-1,-1)
    .to(0,0).curve(
.26,-0.63,.25,1
    )
    .to(1,-1).curve(
.79,0,.61,1.47
    )
).run(x);
```