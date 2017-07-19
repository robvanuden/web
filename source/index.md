---
_disableBreadcrumb: true
_disableContribution: true
_disableAffix: true
_disableContainer: true
jr.disableMetadata: true
jr.disableLeftMenu: true
jr.disableRightMenu: true
uid: index
title: Home
---

<!-- MAIN JUMBOTRON -->
<div class="jumbotron feature">
  <div class="container">
    <div class="row">
        <h1>/njuːk/</h1>
<!--
<style type="text/css">

<![CDATA[

  text {
    filter: url(#filter);
    fill: white;
      font-family: 'Share Tech Mono', sans-serif;
      font-size: 100px;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
        }
]]>
</style>
  <defs>

    <filter id="filter">
        <feFlood flood-color="red" result="flood1" />
        <feFlood flood-color="limegreen" result="flood2" />
      <feOffset in="SourceGraphic" dx="3" dy="0" result="off1a"/>
      <feOffset in="SourceGraphic" dx="2" dy="0" result="off1b"/>
      <feOffset in="SourceGraphic" dx="-3" dy="0" result="off2a"/>
      <feOffset in="SourceGraphic" dx="-2" dy="0" result="off2b"/>
        <feComposite in="flood1" in2="off1a" operator="in"  result="comp1" />
        <feComposite in="flood2" in2="off2a" operator="in" result="comp2" />

        <feMerge x="0" width="100%" result="merge1">
        <feMergeNode in = "black" />
        <feMergeNode in = "comp1" />
        <feMergeNode in = "off1b" />

        <animate 
          attributeName="y" 
            id = "y"
            dur ="4s"
            
            values = '104px; 104px; 30px; 105px; 30px; 2px; 2px; 50px; 40px; 105px; 105px; 20px; 6ßpx; 40px; 104px; 40px; 70px; 10px; 30px; 104px; 102px'

            keyTimes = '0; 0.362; 0.368; 0.421; 0.440; 0.477; 0.518; 0.564; 0.593; 0.613; 0.644; 0.693; 0.721; 0.736; 0.772; 0.818; 0.844; 0.894; 0.925; 0.939; 1'

            repeatCount = "indefinite" />
 
        <animate attributeName="height" 
            id = "h" 
            dur ="4s"
            
            values = '10px; 0px; 10px; 30px; 50px; 0px; 10px; 0px; 0px; 0px; 10px; 50px; 40px; 0px; 0px; 0px; 40px; 30px; 10px; 0px; 50px'

            keyTimes = '0; 0.362; 0.368; 0.421; 0.440; 0.477; 0.518; 0.564; 0.593; 0.613; 0.644; 0.693; 0.721; 0.736; 0.772; 0.818; 0.844; 0.894; 0.925; 0.939; 1'

            repeatCount = "indefinite" />
        </feMerge>
      

      <feMerge x="0" width="100%" y="60px" height="65px" result="merge2">
        <feMergeNode in = "black" />
        <feMergeNode in = "comp2" />
        <feMergeNode in = "off2b" />

        <animate attributeName="y" 
            id = "y"
            dur ="4s"
            values = '103px; 104px; 69px; 53px; 42px; 104px; 78px; 89px; 96px; 100px; 67px; 50px; 96px; 66px; 88px; 42px; 13px; 100px; 100px; 104px;' 

            keyTimes = '0; 0.055; 0.100; 0.125; 0.159; 0.182; 0.202; 0.236; 0.268; 0.326; 0.357; 0.400; 0.408; 0.461; 0.493; 0.513; 0.548; 0.577; 0.613; 1'

            repeatCount = "indefinite" />
 
        <animate attributeName="height" 
            id = "h"
            dur = "4s"
          
          values = '0px; 0px; 0px; 16px; 16px; 12px; 12px; 0px; 0px; 5px; 10px; 22px; 33px; 11px; 0px; 0px; 10px'

            keyTimes = '0; 0.055; 0.100; 0.125; 0.159; 0.182; 0.202; 0.236; 0.268; 0.326; 0.357; 0.400; 0.408; 0.461; 0.493; 0.513;  1'
             
            repeatCount = "indefinite" />
        </feMerge>
      
      <feMerge>
        <feMergeNode in="SourceGraphic" />  

        <feMergeNode in="merge1" /> 
      <feMergeNode in="merge2" />

        </feMerge>
      </filter>

  </defs>

<g>
  <text x="0" y="80">/njuːk/</text>
</g>
</svg>
-->
        <ol class="definition">
          <li>Cross-platform <a href="https://martinfowler.com/articles/continuousIntegration.html#AutomateTheBuild">build automation</a> system with C# DSL.</li>
          <li>Providing magic without actually doing magic.</li>
          <li><a href="https://vimeo.com/221086461">Generating CLI tool support</a> with thousands of lines of boiler-plate code.</li>
        </ol>
        <p>
        <a class="btn btn-primary btn-lg" href="/getting-started.html" role="button">Getting Started</a>
      </div>
  </div>
</div>


<!-- FULL IDE SUPPORT -->
<div class="container feature">
  <div class="row">
    <div class="col-md-5">
      <h2><span class="icon icon-keyboard"></span> Full IDE support</h2>
      <p>Builds are full-fledged C# projects; no magic involved! That means all the powerful IDE features like <a id="auto-completion">auto-completion</a>, refactorings and formatting can celebrate their comback. Targets are defined as expression-bodied properties and therefore provide <a id="navigation">superior navigation</a>. Also target dependency definitions and rename refactorings benefit from <em>targets as properties</em>. Ultimately, <a id="debugging">debugging</a> is available just as you know it. No more writing debug output to the console!</p>
    </div>
    <div class="col-md-7">
      <div id="ide-support-carousel" class="carousel slide" data-ride="carousel1">
        <ol class="carousel-indicators">
          <li data-target="#ide-support-carousel" data-slide-to="0" class="active"></li>
          <li data-target="#ide-support-carousel" data-slide-to="1"></li>
          <li data-target="#ide-support-carousel" data-slide-to="2"></li>
        </ol>
        <div class="carousel-inner" role="listbox">
          <div class="item"><img src="images/completion.png" data-color="lightblue" alt="Auto Completion"></div>
          <div class="item"><img src="images/navigation.png" data-color="firebrick" alt="Navigation"></div>
          <div class="item"><img src="images/debugging.png" data-color="firebrick" alt="Debugging"></div>
        </div>
        <a class="left carousel-control" href="#ide-support-carousel" role="button" data-slide="prev">
          <span class="glyphicon glyphicon-chevron-left" aria-hidden="true"></span>
          <span class="sr-only">Previous</span>
        </a>
        <a class="right carousel-control" href="#ide-support-carousel" role="button" data-slide="next">
          <span class="glyphicon glyphicon-chevron-right" aria-hidden="true"></span>
          <span class="sr-only">Next</span>
        </a>
      </div>
    </div>
  </div>
</div>


<!-- BOOTSTRAPPING JUMBOTRON -->
<div class="jumbotron feature">
  <div class="container">
    <div id="platforms" class="row">
      <h2>Bootstrapping for all platforms included.</h2>
      <span class="icon icon-windows8"></span>
      <span class="icon icon-tux"></span>
      <span class="icon icon-appleinc"></span>
    </div>
  </div>
</div>


<!-- FEATURE LIST -->
<div class="container feature feature-list">
  <div class="row">
    <div class="col-md-6">
      <h2><span class="icon icon-syringe2"></span> Parameter Injection</h2>
      <p>Fields marked with <code>[Parameter]</code> will automatically receive values provided as command-line arguments or environment variables.</p>
      <div class="feature-list-img"><img src="images/parameter-injection.png" alt="Parameter Injection" /></div>
    </div>
    <div class="col-md-6">
      <h2><span class="icon icon-price-tag2"></span> Path Construction</h2>
      <p>Paths can be constructed using the <code>/</code> or <code>+</code> operator, which will take care of creating valid Windows or Unix paths.</p>
      <div class="feature-list-img"><img src="images/path-construction.png" alt="Path Construction" /></div>
    </div>
  </div>
  <div class="row">
    <div class="col-md-6">
      <h2><span class="icon icon-equalizer"></span> Default Settings</h2>
      <p>Common metadata and best-practice settings are automatically loaded into <code>DefaultSettings</code> and can be used to invoke tools.</p>
      <div id="default-settings-carousel" class="carousel slide" data-ride="carousel2">
        <ol class="carousel-indicators">
          <li data-target="#default-settings-carousel" data-slide-to="0" class="active"></li>
          <li data-target="#default-settings-carousel" data-slide-to="1"></li>
        </ol>
        <div class="carousel-inner" role="listbox">
          <div class="item"><img src="images/default-settings01.png" data-color="lightblue" alt="Default Setting Declaration"></div>
          <div class="item"><img src="images/default-settings02.png" data-color="firebrick" alt="Default Setting Usage"></div>
        </div>
        <a class="left carousel-control" href="#default-settings-carousel" role="button" data-slide="prev">
          <span class="glyphicon glyphicon-chevron-left" aria-hidden="true"></span>
          <span class="sr-only">Previous</span>
        </a>
        <a class="right carousel-control" href="#default-settings-carousel" role="button" data-slide="next">
          <span class="glyphicon glyphicon-chevron-right" aria-hidden="true"></span>
          <span class="sr-only">Next</span>
        </a>
      </div>
    </div>
    <div class="col-md-6">
      <h2><span class="icon icon-notebook"></span> Adaptive Logging</h2>
      <p>Log output is optimized for best readability depending on the current environment. For instance, local vs. server build.</p>
      <div id="adaptive-logging-carousel" class="carousel slide" data-ride="carousel3">
        <ol class="carousel-indicators">
          <li data-target="#adaptive-logging-carousel" data-slide-to="0" class="active"></li>
          <li data-target="#adaptive-logging-carousel" data-slide-to="1"></li>
          <li data-target="#adaptive-logging-carousel" data-slide-to="2"></li>
        </ol>
        <div class="carousel-inner" role="listbox">
          <div class="item"><img src="images/logging01.png" data-color="lightblue" alt="Bitrise Logging"></div>
          <div class="item"><img src="images/logging02.png" data-color="lightblue" alt="Console Logging"></div>
          <div class="item"><img src="images/logging03.png" data-color="lightblue" alt="TeamCity Logging"></div>
        </div>
        <a class="left carousel-control" href="#adaptive-logging-carousel" role="button" data-slide="prev">
          <span class="glyphicon glyphicon-chevron-left" aria-hidden="true"></span>
          <span class="sr-only">Previous</span>
        </a>
        <a class="right carousel-control" href="#adaptive-logging-carousel" role="button" data-slide="next">
          <span class="glyphicon glyphicon-chevron-right" aria-hidden="true"></span>
          <span class="sr-only">Next</span>
        </a>
      </div>
    </div>
  </div>
</div>
