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
